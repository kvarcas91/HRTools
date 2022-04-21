using Domain.Models.Meetings;
using Domain.Repository;
using Domain.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Automation
{
    internal sealed class MeetingsAutomation : ITaskAutomation<MeetingsEntity>
    {

        private MeetingsEntity _oldObj;
        private MeetingsEntity _newObj;

        private readonly BaseRepository _repository;
        public MeetingsAutomation(BaseRepository repository)
        {
            _repository = repository;
        }


        public void Invoke(AutomationAction action)
        {
            switch (action)
            {
                case AutomationAction.OnUpdate:
                    //OnUpdate();
                    return;
                default:
                    return;
            }
        }

        public ITaskAutomation<MeetingsEntity> SetData(MeetingsEntity oldObj, MeetingsEntity newObj)
        {
            _oldObj = oldObj;
            _newObj = newObj;
            return this;
        }
    }
}
