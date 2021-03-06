﻿using GoodTimeStudio.OneMinecraftLauncher.Core.Models.Minecraft;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace GoodTimeStudio.OneMinecraftLauncher.UWP
{
    public class MinecraftVersionManager
    {
        public static readonly string VersionManifestUrl = "https://launchermeta.mojang.com/mc/game/version_manifest.json";
        public static readonly string VersionManifestFileName = "version_manifest.json";

        public static MinecraftVersionsList VersionsList;

        public async static void Init()
        {
            VersionsList = await GetMinecraftVersionsFromFileAsync();
            RefreshMinecraftVersions();
        }

        public async static void RefreshMinecraftVersions()
        {
            string json = await GetMinecraftVersionManifestAsync();

            if (!string.IsNullOrWhiteSpace(json))
            {
                MinecraftVersionsList list = GetMinecraftVersions(json);

                if (list != null)
                {
                    VersionsList = list;
                }
            }
        }

        private async static Task<MinecraftVersionsList> GetMinecraftVersionsFromFileAsync()
        {
            StorageFile file = await CoreManager.AppDir.CreateFileAsync(VersionManifestFileName, CreationCollisionOption.OpenIfExists);
            string json = await FileIO.ReadTextAsync(file);

            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            return GetMinecraftVersions(json);
        }

        public static MinecraftVersionsList GetMinecraftVersions(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            try
            {
                VersionsList = JsonConvert.DeserializeObject<MinecraftVersionsList>(json);
                return VersionsList;
            }
            catch (JsonException)
            {
                return null;
            }

        }

        public static List<MinecraftVersion> GetReleaseVersionsList()
        {
            if (VersionsList != null)
            {
                return VersionsList.versions.Where(t => t.type == "release").ToList();
            }
            return null;
        }

        /// <summary>
        /// Get minecraft version manifest.
        /// </summary>
        /// <returns>version manifest json</returns>
        private async static Task<string> GetMinecraftVersionManifestAsync()
        {
            string json;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    json = await client.GetStringAsync(VersionManifestUrl);
                }
            }
            catch (HttpRequestException)
            {
                return string.Empty;
            }

            return json;
        }
    }

}
