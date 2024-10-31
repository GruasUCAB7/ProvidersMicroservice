﻿using ProvidersMS.Core.Domain.Aggregates;
using ProvidersMS.src.Cranes.Domain.Events;
using ProvidersMS.src.Cranes.Domain.Exceptions;
using ProvidersMS.src.Cranes.Domain.ValueObjects;

namespace ProvidersMS.src.Cranes.Domain
{
    public class Crane(CraneId id) : AggregateRoot<CraneId>(id)
    {
        private CraneId _id = id;
        private CraneBrand _brand;
        private CraneModel _model;
        private CranePlate _plate;
        private CraneSizeType _craneType;
        private CraneYear _year;
        private bool _isActive = true;
        private DateTime _creationDate = DateTime.UtcNow;

        public string GetId() => _id.GetValue();
        public string GetBrand() => _brand.GetValue();
        public string GetModel() => _model.GetValue();
        public string GetPlate() => _plate.GetValue();
        public string GetCraneType() => _craneType.GetValue();
        public int GetYear() => _year.GetValue();
        public bool GetIsActive() => _isActive;
        public DateTime GetCreationDate() => _creationDate;
        public void SetBrand(string brand) => _brand = new CraneBrand(brand);
        public void SetModel(string model) => _model = new CraneModel(model);
        public void SetPlate(string plate) => _plate = new CranePlate(plate);
        public void SetCraneType(string craneType) => _craneType = Enum.Parse<CraneSizeType>(craneType);
        public void SetYear(int year) => _year = new CraneYear(year);
        public bool SetIsActive(bool isActive) => _isActive = isActive;


        public static Crane CreateCrane(CraneId id, CraneBrand brand, CraneModel model, CranePlate plate, CraneSizeType craneType, CraneYear year)
        {
            var crane = new Crane(id);
            crane.Apply(CraneCreated.CreateEvent(id, brand, model, plate, craneType, year));
            return crane;
        }

        public void OnCraneCreatedEvent(CraneCreated context)
        {
            _id = new CraneId(context.Id);
            _brand = new CraneBrand(context.Brand);
            _model = new CraneModel(context.Model);
            _plate = new CranePlate(context.Plate);
            _craneType = Enum.Parse<CraneSizeType>(context.CraneType);
            _year = new CraneYear(context.Year);
        }

        public void OnCraneUpdatedEvent(CraneUpdated context)
        {
            if (context.IsActive != null)
            {
                _isActive = context.IsActive.Value;
            }
        }

        public override void ValidateState()
        {
            if (_id == null || _brand == null || _model == null || _plate == null || _craneType == null || _year == null)
            {
                throw new InvalidCraneException();
            }
        }
    }
}
