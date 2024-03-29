﻿using AutoMapper;
using ElectronicMenu.BL.Positions.Entities;
using ElectronicMenu.DataAccess;
using ElectronicMenu.DataAccess.Entities;

namespace ElectronicMenu.BL.Positions
{
    public class PositionsProvider : IPositionsProvider
    {
        private readonly IRepository<PositionEntity> _positionsRepository;
        private readonly IMapper _mapper;

        public PositionsProvider(IRepository<PositionEntity> positionsRepository, IMapper mapper)
        {
            _positionsRepository = positionsRepository;
            _mapper = mapper;
        }

        public PositionModel GetPosition(int positionId)
        {
            PositionEntity? position = _positionsRepository.GetById(positionId);

            if (position == null)
            {
                throw new ArgumentException("Нет позиции по заданному id");
            }

            return _mapper.Map<PositionModel>(position);
        }

        public IEnumerable<PositionModel> GetPositions(PositionModelFilter? filter = null)
        {
            float? minPrice = filter?.MinPrice;
            float? maxPrice = filter?.MaxPrice;

            float? minWeight = filter?.MinWeight;
            float? maxWeight = filter?.MaxWeight;

            float? minCalories = filter?.MinCalories;
            float? maxCalories = filter?.MaxCalories;

            IEnumerable<PositionEntity> positions = _positionsRepository.GetAll(x => (
                (minPrice == null || x.Price >= minPrice) &&
                (maxPrice == null || x.Price <= maxPrice) &&
                (minWeight == null || x.Weight >= minWeight) &&
                (maxWeight == null || x.Weight <= maxWeight) &&
                (minCalories == null || x.Calories >= minCalories) &&
                (maxCalories == null || x.Calories <= maxCalories)
               ));

            return _mapper.Map<IEnumerable<PositionModel>>(positions);
        }

        public IEnumerable<PositionModel> GetFilteredSortedPagePositions(GetFilteredSortedPagePositionModel? rules = null)
        {
            float? minPrice = rules?.MinPrice;
            float? maxPrice = rules?.MaxPrice;

            float? minWeight = rules?.MinWeight;
            float? maxWeight = rules?.MaxWeight;

            float? minCalories = rules?.MinCalories;
            float? maxCalories = rules?.MaxCalories;

            IEnumerable<PositionEntity> positions = _positionsRepository.GetAll(x => (
                (minPrice == null || x.Price >= minPrice) &&
                (maxPrice == null || x.Price <= maxPrice) &&
                (minWeight == null || x.Weight >= minWeight) &&
                (maxWeight == null || x.Weight <= maxWeight) &&
                (minCalories == null || x.Calories >= minCalories) &&
                (maxCalories == null || x.Calories <= maxCalories)
               ));

            if (rules.SortDirection)
                positions = positions.OrderByDescending(x => x.GetType().GetProperty(rules.SortField)?.GetValue(x) ?? null);
            else
                positions = positions.OrderBy(x => x.GetType().GetProperty(rules.SortField)?.GetValue(x) ?? null);

            positions = positions.Skip((rules.Page - 1) * rules.PageSize).Take(rules.PageSize);

            return _mapper.Map<IEnumerable<PositionModel>>(positions);
        }
    }
}
