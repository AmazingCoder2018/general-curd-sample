﻿using Volo.Abp.Application.Dtos;
using System;
using Application.Share.Services;

namespace Matoapp.Health.Record.Dto
{
    public class GetAllValueRecordInput : 
        PagedAndSortedResultRequestDto, 
        IUserOrientedFilter, 
        IOrganizationOrientedFilter, 
        IRelationToOrientedFilter, 
        IDateSpanOrientedFilter,
        IKeywordOrientedFilter
    {

        //keyword
        public string Keyword { get; set; }
        public string TargetFields { get; set; }
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Status { get; set; }



        //医生-患者视图相关
        public Guid? UserId { get; set; }
        public Guid? OrganizationUnitId { get; set; }
        public Guid? RelationToUserId { get; set; }
        public string RelationType { get; set; }

        public string EntityUserIdIdiom { get; }
    }
}
