/*
 *  This file is part of .Net eID Client.
 *  Copyright (C) 2014 Egelke BVBA
 *
 *  .Net eID Client is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Lesser General Public License as published by
 *  the Free Software Foundation, either version 2.1 of the License, or
 *  (at your option) any later version.
 *
 *  .Net eID Client is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with .Net eID Client.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Egelke.Eid.Client
{
    public class DeviceEventArgs : EventArgs
    {
        private String deviceName;
        private DeviceState previousState;
        private DeviceState newState;

        public String DeviceName
        {
            get
            {
                return deviceName;
            }
        }

        public DeviceState PreviousState
        {
            get
            {
                return previousState;
            }
        }

        public DeviceState NewState
        {
            get
            {
                return newState;
            }
        }

        internal DeviceEventArgs(String deviceName, DeviceState previousState, DeviceState newState)
        {
            this.deviceName = deviceName;
            this.previousState = previousState;
            this.newState = newState;
        }

    }
}
