using System;
using System.Collections.Generic;

namespace Shop.Model
{
    class Showcase : Entity
    {
        public string Name { get; set; }
        public int MaxCapacity { get; set; }
        public int Capacity { get; private set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? RemovedAt { get; set; }

        public override IResult Validate()
        {
            var result = new Result(true);

            if (string.IsNullOrWhiteSpace(Name))
            {
                result.Success = false;
                result.Message += "Наименование не должно быть пустым\r\n";
            }

            if (MaxCapacity < 1)
            {
                result.Success = false;
                result.Message += "Максимально допустимый объем должен быть целым положительным числом\r\n";
            }

            return result;
        }

        public override string ToString() 
            =>$"[{Id}] {Name} (cap: {Capacity}/{MaxCapacity}) от " + CreatedAt.ToShortDateString() + ((RemovedAt != null) ? "(удалено " + RemovedAt?.ToShortDateString() + ")" : "");
    }
}