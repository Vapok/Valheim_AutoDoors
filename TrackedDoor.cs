using AutoDoors.GameClasses;

namespace AutoDoors
{
    public class TrackedDoor
    {
        public int Id { get; private set; }
        public int State { get; private set; }
        public bool IsValid { get; private set; } = true;

        public bool InAutoRange { get; set; }
        public bool IsManual { get; set; }
        public bool IsAutoOpened { get; set; }

        public TrackedDoor(int id)
        {
            Id = id;
            Update();
            IsManual = State != 0;
        }

        public bool Update()
        {
            var door = Door_UpdateState_Patch.DoorCache[Id];
            if (door != null)
            {
                var zd0 = door.m_nview.GetZDO();
                if (zd0 != null)
                {
                    IsValid = true;
                    State = door.m_nview.GetZDO().GetInt("state", 0);
                }
                else
                    IsValid = false;
            }
            else
            {
                IsValid = false;
            }
            
            return IsValid;
        }

        public void SetState(int newState)
        {
            var door = Door_UpdateState_Patch.DoorCache[Id];
            var zd0 = door.m_nview.GetZDO();
            if (zd0 != null)
                zd0.Set("state", newState);
        }
    }
}
