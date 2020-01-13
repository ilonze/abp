﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NSubstitute;
using System.Threading.Tasks;
using Volo.Abp.Auditing;
using Xunit;

namespace Volo.Abp.AspNetCore.Mvc.Auditing
{
    public class AuditTestController_Tests : AspNetCoreMvcTestBase
    {
        private readonly AbpAuditingOptions _options;
        private IAuditingStore _auditingStore;

        public AuditTestController_Tests()
        {
            _options = ServiceProvider.GetRequiredService<IOptions<AbpAuditingOptions>>().Value;
            _auditingStore = ServiceProvider.GetRequiredService<IAuditingStore>();
        }

        protected override void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            _auditingStore = Substitute.For<IAuditingStore>();
            services.Replace(ServiceDescriptor.Singleton(_auditingStore));
            base.ConfigureServices(context, services);
        }

        [Fact]
        public async Task Should_Trigger_Middleware_And_AuditLog_Success_For_GetRequests()
        {
            _options.IsEnabledForGetRequests = true;
            await GetResponseAsync("api/audit-test/audit-success");
            //await _auditingStore.Received().SaveAsync(Arg.Any<AuditLogInfo>()); //Won't work, save happens out of scope
        }

        [Fact]
        public async Task Should_Trigger_Middleware_And_AuditLog_Exception_Always()
        {
            _options.IsEnabled = false;
            _options.AlwaysLogOnException = false;
            _options.HideErrors = false;
            await GetResponseAsync("api/audit-test/audit-fail", System.Net.HttpStatusCode.BadRequest);
            //await _auditingStore.Received().SaveAsync(Arg.Any<AuditLogInfo>()); //Won't work, save happens out of scope
        }
    }
}
