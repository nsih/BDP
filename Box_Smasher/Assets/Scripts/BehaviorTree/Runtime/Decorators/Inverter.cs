﻿using BS.BehaviorTrees.Tasks;

namespace BS.BehaviorTrees.Decorators
{
    public class Inverter : DecoratorBase
    {
        protected override TaskStatus OnUpdate()
        {
            var childStatus = Child.Update();
            var status = childStatus;

            switch(childStatus)
            {
                case TaskStatus.Success:
                    status = TaskStatus.Failure;
                    break;
                case TaskStatus.Failure:
                    status = TaskStatus.Success;
                    break;
            }

            return status;
        }
    }
}