﻿using Matoapp.Common.Application.DataEnumCategory.Dto;
using System;
using Volo.Abp.Application.Dtos;

namespace Matoapp.Common.Application.DataEnums.Dtos
{
    public class DataEnumDto : FullAuditedEntityDto<long>
    {
        public DataEnumCategoryDto DataEnumCategory { get; set; }

        public long DataEnumCategoryId { get; set; }

        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }


        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }


        /// <summary>
        /// 系统配置
        /// </summary>
        public bool IsSys { get; set; }
    }
}

