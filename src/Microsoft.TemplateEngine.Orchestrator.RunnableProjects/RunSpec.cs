using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.TemplateEngine.Abstractions;
using Microsoft.TemplateEngine.Core;
using Microsoft.TemplateEngine.Core.Contracts;
using Microsoft.TemplateEngine.Orchestrator.RunnableProjects.Config;

namespace Microsoft.TemplateEngine.Orchestrator.RunnableProjects
{
    internal class RunSpec : IRunSpec, IServiceContainer
    {
        private readonly IReadOnlyList<IOperationProvider> _overrides;
        private readonly IVariableCollection _vars;
        private readonly IComponentManager _componentManager;
        private readonly ISet<Guid> _services;

        public RunSpec(IComponentManager componentManager, IReadOnlyList<Guid> services, IReadOnlyList<IOperationProvider> operationOverrides, IVariableCollection vars, string variableFormatString)
        {
            _services = new HashSet<Guid>(services);
            _componentManager = componentManager;
            _overrides = operationOverrides;
            _vars = vars ?? new VariableCollection();
            VariableFormatString = variableFormatString ?? "{0}";
        }

        public bool TryGetTargetRelPath(string sourceRelPath, out string targetRelPath)
        {
            targetRelPath = null;
            return false;
        }

        public IReadOnlyList<IOperationProvider> GetOperations(IReadOnlyList<IOperationProvider> sourceOperations)
        {
            return _overrides ?? sourceOperations;
        }

        public IVariableCollection ProduceCollection(IVariableCollection parent)
        {
            return _vars;
        }

        public string VariableFormatString { get; }

        public bool TryGetService<T>(out T service)
            where T : class, IIdentifiedComponent
        {
            IReadOnlyList<T> services = GetServices<T>();

            if (services.Count != 1)
            {
                service = null;
                return false;
            }

            service = services[0];
            return true;
        }

        public IReadOnlyList<T> GetServices<T>()
            where T : class, IIdentifiedComponent
        {
            return _componentManager.OfType<T>().Where(x => _services.Contains(x.Id)).ToList();
        }
    }
}
