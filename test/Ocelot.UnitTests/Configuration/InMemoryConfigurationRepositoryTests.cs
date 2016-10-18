﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ocelot.Library.Configuration;
using Ocelot.Library.Configuration.Builder;
using Ocelot.Library.Configuration.Repository;
using Ocelot.Library.Responses;
using Shouldly;
using TestStack.BDDfy;
using Xunit;

namespace Ocelot.UnitTests.Configuration
{
    public class InMemoryConfigurationRepositoryTests
    {
        private readonly InMemoryOcelotConfigurationRepository _repo;
        private IOcelotConfiguration _config;
        private Response _result;
        private Response<IOcelotConfiguration> _getResult;

        public InMemoryConfigurationRepositoryTests()
        {
            _repo = new InMemoryOcelotConfigurationRepository();
        }

        [Fact]
        public void can_add_config()
        {
            this.Given(x => x.GivenTheConfigurationIs(new FakeConfig("initial")))
                .When(x => x.WhenIAddOrReplaceTheConfig())
                .Then(x => x.ThenNoErrorsAreReturned())
                .BDDfy();
        }

        [Fact]
        public void can_get_config()
        {
            this.Given(x => x.GivenThereIsASavedConfiguration())
                .When(x => x.WhenIGetTheConfiguration())
                .Then(x => x.ThenTheConfigurationIsReturned())
                .BDDfy();
        }

        /// <summary>
        /// long runnnig unit test to make sure repo thread safeok on
        /// </summary>
        [Fact]
        public void repo_is_thread_safe()
        {
            var tasks = new Task[100000];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Fire();
            }

            Task.WaitAll(tasks);
        }

        private async Task Fire()
        {
            var taskGuid = Guid.NewGuid().ToString();
            _repo.AddOrReplace(new FakeConfig(taskGuid));
            var configuration = _repo.Get();
            configuration.Data.ReRoutes[0].DownstreamTemplate.ShouldBe(taskGuid);
        }

        private void ThenTheConfigurationIsReturned()
        {
            _getResult.Data.ReRoutes[0].DownstreamTemplate.ShouldBe("initial");
        }

        private void WhenIGetTheConfiguration()
        {
            _getResult = _repo.Get();
        }

        private void GivenThereIsASavedConfiguration()
        {
            GivenTheConfigurationIs(new FakeConfig("initial"));
            WhenIAddOrReplaceTheConfig();
        }

        private void GivenTheConfigurationIs(IOcelotConfiguration config)
        {
            _config = config;
        }

        private void WhenIAddOrReplaceTheConfig()
        {
            _result = _repo.AddOrReplace(_config);
        }

        private void ThenNoErrorsAreReturned()
        {
            _result.IsError.ShouldBeFalse();
        }

        class FakeConfig : IOcelotConfiguration
        {
            private readonly string _downstreamTemplate;

            public FakeConfig(string downstreamTemplate)
            {
                _downstreamTemplate = downstreamTemplate;
            }

            public List<ReRoute> ReRoutes => new List<ReRoute>
            {
                new ReRouteBuilder().WithDownstreamTemplate(_downstreamTemplate).Build()
            };
        }
    }
}