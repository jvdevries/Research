using System.Configuration;
using System;
using System.Diagnostics;

namespace DevUtil.Aspects.Settings
{
    public class SettingsManager : ConfigurationSection
    {
        protected SettingsManager()
        {

        }

        /// <summary>
        /// Saves the configuration to the default file.
        /// </summary>
        public void Save()
        {
            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                if (config.Sections[GetType().Name] == null)
                    config.Sections.Add(GetType().Name, this);

                SectionInformation.ForceSave = true;
                config.Save(ConfigurationSaveMode.Minimal);
            }
            catch
            {
                // Can't do anything.
            }
        }

        /// <summary>
        /// Loads a <see cref="SettingsBase"/> derived class from the configuration file.
        /// </summary>
        /// <typeparam name="T">A <see cref="SettingsBase"/> derived class.</typeparam>
        /// <returns>An instance of the <see cref="SettingsBase"/> derived class.</returns>
        public static T Load<T>() where T : SettingsManager
        {
            T settings = null;
            try
            {
                settings = Activator.CreateInstance(typeof(T)) as T;
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                if (!(config.GetSection(typeof(T).Name) is T loaded))
                {
                    settings.Save();
                    return settings;
                }

                return loaded;
            }
            catch
            {
                return settings;
            }
        }
    }
}