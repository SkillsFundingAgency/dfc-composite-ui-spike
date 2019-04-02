﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace Ncs.Prototype.Web.Composition.HealthCheck
{
    public class ReadinessPublisher : IHealthCheckPublisher
    {
        private readonly ILogger<ReadinessPublisher> _logger;

        public ReadinessPublisher(ILogger<ReadinessPublisher> logger)
        {
            _logger = logger;
        }

        public List<(HealthReport report, CancellationToken cancellationToken)> Entries { get; } = new List<(HealthReport report, CancellationToken cancellationToken)>();

        public Exception Exception { get; set; }

        public Task PublishAsync(HealthReport report, CancellationToken cancellationToken)
        {
            Entries.Add((report, cancellationToken));

            _logger.LogInformation("{TIMESTAMP} Readiness Probe Status: {RESULT}", DateTime.UtcNow, report.Status);

            if (Exception != null)
            {
                throw Exception;
            }

            cancellationToken.ThrowIfCancellationRequested();

            return Task.CompletedTask;
        }
    }
}
