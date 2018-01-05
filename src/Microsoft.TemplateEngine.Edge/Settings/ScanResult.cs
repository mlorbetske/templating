using System.Collections.Generic;
using Microsoft.TemplateEngine.Abstractions;

namespace Microsoft.TemplateEngine.Edge.Settings
{
    public class ScanResult
    {
        public ScanResult()
        {
            _localizations = new List<ILocalizationLocator>();
            _templates = new List<ITemplate>();
            _installedItems = new List<ScanResultEntry>();
        }

        private List<ILocalizationLocator> _localizations;
        private List<ITemplate> _templates;
        private List<ScanResultEntry> _installedItems;

        public void AddLocalization(ILocalizationLocator locator)
        {
            _localizations.Add(locator);
        }

        public void AddTemplate(ITemplate template)
        {
            _templates.Add(template);
        }

        public void AddInstalledMountPointId(ScanResultEntry entry)
        {
            _installedItems.Add(entry);
        }

        public IReadOnlyList<ILocalizationLocator> Localizations => _localizations;

        public IReadOnlyList<ITemplate> Templates => _templates;

        public IReadOnlyList<ScanResultEntry> InstalledItems => _installedItems;
    }
}
