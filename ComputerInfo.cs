using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkListening
{
    public class ComputerInfo
    {
        private static string _computerName;
        /// <summary>
        /// 计算机名称
        /// </summary>
        public static string ComputerName
        {
            get
            {
                return _computerName;
            }

            set
            {
                _computerName = value;
            }
        }
    }
}
