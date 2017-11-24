﻿using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace KenticoCloud.ContentManagement.Models.Assets
{
    public class AssetUpdateModel
    {
        [JsonProperty("descriptions", Required = Required.Always)]
        public IEnumerable<AssetDescriptionsModel> Descriptions { get; set; }
    }
}
