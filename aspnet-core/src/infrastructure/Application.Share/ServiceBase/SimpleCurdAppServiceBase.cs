﻿using System;
using System.Linq;
using System.Linq.Expressions;
using Volo.Abp.Application.Services;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Application.Share.Dto;
using System.Threading.Tasks;
using System.Collections.Generic;
using Volo.Abp.ObjectMapping;
using Application.Share.Services;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Users;

namespace Application.Share.ServiceBase
{
    public abstract class SimpleCurdAppServiceBase<TEntity, TEntityDto, TKey>
        : SimpleCurdAppServiceBase<TEntity, TEntityDto, TKey, PagedAndSortedResultRequestDto>
        where TEntity : class, IEntity<TKey>
        where TEntityDto : IEntityDto<TKey>
    {
        protected SimpleCurdAppServiceBase(IRepository<TEntity, TKey> repository)
            : base(repository)
        {

        }
    }

    public abstract class SimpleCurdAppServiceBase<TEntity, TEntityDto, TKey, TGetListInput>
        : SimpleCurdAppServiceBase<TEntity, TEntityDto, TKey, TGetListInput, TEntityDto>
        where TEntity : class, IEntity<TKey>
        where TEntityDto : IEntityDto<TKey>
    {
        protected SimpleCurdAppServiceBase(IRepository<TEntity, TKey> repository)
            : base(repository)
        {

        }
    }


    public abstract class SimpleCurdAppServiceBase<TEntity, TEntityDto, TKey, TGetListInput, TCreateInput>
     : SimpleCurdAppServiceBase<TEntity, TEntityDto, TKey, TGetListInput, TCreateInput, TCreateInput>
     where TEntity : class, IEntity<TKey>
     where TEntityDto : IEntityDto<TKey>
    {
        protected SimpleCurdAppServiceBase(IRepository<TEntity, TKey> repository)
            : base(repository)
        {

        }
    }

    public abstract class SimpleCurdAppServiceBase<TEntity, TEntityDto, TKey, TGetListInput, TCreateInput, TUpdateInput>
    : SimpleCurdAppServiceBase<TEntity, TEntityDto, TEntityDto, TKey, TGetListInput, TCreateInput, TUpdateInput>
    where TEntity : class, IEntity<TKey>
    where TEntityDto : IEntityDto<TKey>
    {
        protected SimpleCurdAppServiceBase(IRepository<TEntity, TKey> repository)
            : base(repository)
        {

        }

        protected override Task<TEntityDto> MapToGetListOutputDtoAsync(TEntity entity)
        {
            return MapToGetOutputDtoAsync(entity);
        }

        protected override TEntityDto MapToGetListOutputDto(TEntity entity)
        {
            return MapToGetOutputDto(entity);
        }
    }


    public abstract class SimpleCurdAppServiceBase<TEntity, TGetOutputDto, TGetListOutputDto, TKey, TGetListInput, TCreateInput, TUpdateInput>
        : CrudAppService<TEntity, TGetOutputDto, TGetListOutputDto, TKey, TGetListInput, TCreateInput, TUpdateInput>
        where TEntity : class, IEntity<TKey>
            where TGetOutputDto : IEntityDto<TKey>
    where TGetListOutputDto : IEntityDto<TKey>
    {
        protected SimpleCurdAppServiceBase(IRepository<TEntity, TKey> repository)
    : base(repository)
        {

        }


        private new Task<TGetOutputDto> UpdateAsync(TKey id, TUpdateInput input)
        {
            return base.UpdateAsync(id, input);
        }
        private new Task<PagedResultDto<TGetListOutputDto>> GetListAsync(TGetListInput input)
        {
            return base.GetListAsync(input);
        }

        public virtual async Task<TGetOutputDto> UpdateAsync(TUpdateInput input)
        {
            await CheckUpdatePolicyAsync();
            var entity = await GetEntityByIdAsync((input as IEntityDto<TKey>).Id);
            MapToEntity(input, entity);
            await Repository.UpdateAsync(entity, autoSave: true);
            return await MapToGetOutputDtoAsync(entity);

        }
        public virtual Task<PagedResultDto<TGetListOutputDto>> GetAllAsync(TGetListInput input)
        {
            return this.GetListAsync(input);
        }


        protected override async Task<IQueryable<TEntity>> CreateFilteredQueryAsync(TGetListInput input)
        {
            var query = await ReadOnlyRepository.GetQueryableAsync();

            query = await DefaultConvention(input, query);

            return query;
        }

        protected virtual async Task<IQueryable<TEntity>> DefaultConvention(TGetListInput input, IQueryable<TEntity> query)
        {
            query = ApplySearchFiltered(query, input);
            query = ApplyUserOrientedFiltered(query, input);
            query = await ApplyOrganizationOrientedFiltered(query, input);
            query = await ApplyRelationToOrientedFiltered(query, input);
            query = await ApplyRelationFromOrientedFiltered(query, input);
            return query;
        }


        protected virtual IQueryable<TEntity> ApplyUserOrientedFiltered(IQueryable<TEntity> query, TGetListInput input)
        {
            if (input is IUserOrientedFilter && HasProperty<TEntity>("UserId"))
            {
                var property = typeof(TEntity).GetProperty("UserId");
                var filteredInput = input as IUserOrientedFilter;
                if (filteredInput != null && filteredInput.UserId.HasValue)
                {
                    Guid userId = default;
                    if (filteredInput.UserId.Value == Guid.Empty)
                    {
                        using (var scope = ServiceProvider.CreateScope())
                        {
                            var currentUser = scope.ServiceProvider.GetRequiredService<ICurrentUser>();
                            if (currentUser != null)
                            {
                                userId = currentUser.GetId();
                            }
                        }
                    }
                    else
                    {
                        userId = filteredInput.UserId.Value;
                    }

                    var parameter = Expression.Parameter(typeof(TEntity), "p");
                    var keyConstantExpression = Expression.Constant(userId, typeof(Guid));

                    var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                    var expression = Expression.Equal(propertyAccess, keyConstantExpression);

                    var equalExpression = expression != null ?
                         Expression.Lambda<Func<TEntity, bool>>(expression, parameter)
                         : p => false;

                    query = query.Where(equalExpression);
                }

            }
            return query;
        }

        protected virtual Task<IEnumerable<Guid>> GetUserIdsByRelatedToAsync(Guid userId, string relationType)
        {
            return Task.FromResult((IEnumerable<Guid>)new List<Guid>());
        }

        protected virtual Task<IEnumerable<Guid>> GetUserIdsByRelatedFromAsync(Guid userId, string relationType)
        {
            return Task.FromResult((IEnumerable<Guid>)new List<Guid>());
        }


        protected virtual Task<IEnumerable<Guid>> GetUserIdsByOrganizationAsync(Guid organizationUnitId)
        {
            return Task.FromResult((IEnumerable<Guid>)new List<Guid>());
        }


        protected virtual async Task<IQueryable<TEntity>> ApplyOrganizationOrientedFiltered(IQueryable<TEntity> query, TGetListInput input)
        {
            if (input is IOrganizationOrientedFilter && HasProperty<TEntity>("UserId"))
            {
                var property = typeof(TEntity).GetProperty("UserId");
                var filteredInput = input as IOrganizationOrientedFilter;
                if (filteredInput != null && filteredInput.OrganizationUnitId.HasValue)
                {

                    var ids = await GetUserIdsByOrganizationAsync(filteredInput.OrganizationUnitId.Value);
                    Expression originalExpression = null;
                    var parameter = Expression.Parameter(typeof(TEntity), "p");
                    foreach (var id in ids)
                    {
                        var keyConstantExpression = Expression.Constant(id, typeof(Guid));
                        var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                        var expressionSegment = Expression.Equal(propertyAccess, keyConstantExpression);

                        if (originalExpression == null)
                        {
                            originalExpression = expressionSegment;
                        }
                        else
                        {
                            originalExpression = Expression.Or(originalExpression, expressionSegment);
                        }
                    }

                    var equalExpression = originalExpression != null ?
                         Expression.Lambda<Func<TEntity, bool>>(originalExpression, parameter)
                         : p => false;

                    query = query.Where(equalExpression);

                }

            }
            return query;
        }


        protected virtual async Task<IQueryable<TEntity>> ApplyRelationToOrientedFiltered(IQueryable<TEntity> query, TGetListInput input)
        {
            if (input is IRelationToOrientedFilter && HasProperty<TEntity>("UserId"))
            {
                var property = typeof(TEntity).GetProperty("UserId");
                var filteredInput = input as IRelationToOrientedFilter;
                if (filteredInput != null && filteredInput.RelationToUserId.HasValue && !string.IsNullOrEmpty(filteredInput.RelationType))
                {

                    Guid userId = default;
                    if (filteredInput.RelationToUserId.Value == Guid.Empty)
                    {
                        using (var scope = ServiceProvider.CreateScope())
                        {
                            var currentUser = scope.ServiceProvider.GetRequiredService<ICurrentUser>();
                            if (currentUser != null)
                            {
                                userId = currentUser.GetId();
                            }
                        }
                    }
                    else
                    {
                        userId = filteredInput.RelationToUserId.Value;
                    }

                    var ids = await GetUserIdsByRelatedToAsync(userId, filteredInput.RelationType);
                    Expression originalExpression = null;
                    var parameter = Expression.Parameter(typeof(TEntity), "p");
                    foreach (var id in ids)
                    {
                        var keyConstantExpression = Expression.Constant(id, typeof(Guid));
                        var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                        var expressionSegment = Expression.Equal(propertyAccess, keyConstantExpression);

                        if (originalExpression == null)
                        {
                            originalExpression = expressionSegment;
                        }
                        else
                        {
                            originalExpression = Expression.Or(originalExpression, expressionSegment);
                        }
                    }

                    var equalExpression = originalExpression != null ?
                         Expression.Lambda<Func<TEntity, bool>>(originalExpression, parameter)
                         : p => false;

                    query = query.Where(equalExpression);

                }

            }
            return query;
        }


        protected virtual async Task<IQueryable<TEntity>> ApplyRelationFromOrientedFiltered(IQueryable<TEntity> query, TGetListInput input)
        {
            if (input is IRelationFromOrientedFilter && HasProperty<TEntity>("UserId"))
            {
                var property = typeof(TEntity).GetProperty("UserId");
                var filteredInput = input as IRelationFromOrientedFilter;
                if (filteredInput != null && filteredInput.RelationFromUserId.HasValue && !string.IsNullOrEmpty(filteredInput.RelationType))
                {

                    Guid userId = default;
                    if (filteredInput.RelationFromUserId.Value == Guid.Empty)
                    {
                        using (var scope = ServiceProvider.CreateScope())
                        {
                            var currentUser = scope.ServiceProvider.GetRequiredService<ICurrentUser>();
                            if (currentUser != null)
                            {
                                userId = currentUser.GetId();
                            }
                        }
                    }
                    else
                    {
                        userId = filteredInput.RelationFromUserId.Value;
                    }

                    var ids = await GetUserIdsByRelatedFromAsync(userId, filteredInput.RelationType);
                    Expression originalExpression = null;
                    var parameter = Expression.Parameter(typeof(TEntity), "p");
                    foreach (var id in ids)
                    {
                        var keyConstantExpression = Expression.Constant(id, typeof(Guid));
                        var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                        var expressionSegment = Expression.Equal(propertyAccess, keyConstantExpression);

                        if (originalExpression == null)
                        {
                            originalExpression = expressionSegment;
                        }
                        else
                        {
                            originalExpression = Expression.Or(originalExpression, expressionSegment);
                        }
                    }

                    var equalExpression = originalExpression != null ?
                         Expression.Lambda<Func<TEntity, bool>>(originalExpression, parameter)
                         : p => false;

                    query = query.Where(equalExpression);

                }

            }
            return query;
        }


        protected virtual bool HasProperty<T>(string propertyName)
        {
            return typeof(T).GetProperty(propertyName) != null;
        }

        protected virtual bool HasProperty(TEntity entity, string propertyName)
        {
            return entity.GetType().GetProperty(propertyName) != null;
        }


        protected virtual bool HasProperty(Type type, string propertyName)
        {
            return type.GetProperty(propertyName) != null;
        }



        protected virtual IQueryable<TEntity> ApplySearchFiltered(IQueryable<TEntity> query, TGetListInput input)
        {
            if (input is IKeywordOrientedFilter)
            {
                var filteredInput = input as IKeywordOrientedFilter;
                if (filteredInput != null)
                {
                    var targetFields = new string[] { "Name", "Title" };
                    if (!string.IsNullOrEmpty(filteredInput.TargetFields))
                    {
                        targetFields = filteredInput.TargetFields.Split(',');
                    }

                    return query.WhereIf(!filteredInput.Keyword.IsNullOrWhiteSpace(),
                        FilterByKeywordDynamic<TEntity>(filteredInput.Keyword, targetFields));
                }
            }
            return query;
        }

        private Expression<Func<TEntity, bool>> FilterByKeywordDynamic<T>(string keyword, params string[] sortColumns)
        {
            var parameter = Expression.Parameter(typeof(T), "p");
            var propertys = sortColumns.Select(sortColumn => typeof(T).GetProperty(sortColumn));

            var method = typeof(string)
                .GetMethods()
                .FirstOrDefault(x => x.Name == "Contains");

            var keyConstantExpression = Expression.Constant(keyword, typeof(string));
            Expression originalExpression = null;
            foreach (var property in propertys)
            {
                if (property != null)
                {
                    var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                    var expression = Expression.Call(propertyAccess, method, keyConstantExpression);
                    if (originalExpression == null)
                    {
                        originalExpression = expression;
                    }
                    else
                    {
                        originalExpression = Expression.Or(originalExpression, expression);
                    }
                }
            }

            var result = originalExpression != null ?
                 Expression.Lambda<Func<TEntity, bool>>(originalExpression, parameter)
                 : p => true;
            return result;


        }

    }



}