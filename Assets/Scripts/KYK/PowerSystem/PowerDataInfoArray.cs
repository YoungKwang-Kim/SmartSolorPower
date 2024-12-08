using System;
using System.Collections.Generic;
using UnityEngine;

namespace PowerPlant.Data
{
    /// <summary>
    /// �������� ��ü ���� �����͸� ��� �����̳� Ŭ����
    /// </summary>
    [Serializable]
    public class PowerDataInfoArray
    {
        public List<PowerData> powerDataInfo;
    }

    /// <summary>
    /// ���� �������� ���� ���� �� ��� �����͸� ��Ÿ���� Ŭ����
    /// </summary>
    [Serializable]
    public class PowerData
    {
        #region Production Data
        [Tooltip("���� ������ (MWh)")]
        public double dayGelec;

        [Tooltip("���� ������ (MWh)")]
        public double accumGelec;
        #endregion

        #region Consumption Data
        [Tooltip("���� ��뷮 (MWh)")]
        public double dayPrdct;

        [Tooltip("���� ��뷮 (MWh)")]
        public double cntuAccumPowerPrdct;
        #endregion

        #region Environmental Data
        [Tooltip("���� CO2 ������ (��)")]
        public int co2;

        [Tooltip("�ð��� CO2 ������ (��/��)")]
        public double hco2;
        #endregion

        /// <summary>
        /// ���� ȿ���� ����մϴ�.
        /// </summary>
        /// <returns>���� ȿ�� (�����)</returns>
        public double CalculateEfficiency()
        {
            if (dayPrdct == 0) return 0;
            return (dayGelec / dayPrdct) * 100;
        }

        /// <summary>
        /// CO2 ���� ȿ���� ����մϴ�.
        /// </summary>
        /// <returns>�ð��� ��� CO2 ������</returns>
        public double CalculateAverageCO2Reduction()
        {
            const int HOURS_PER_DAY = 24;
            return co2 / (double)HOURS_PER_DAY;
        }
    }

    /// <summary>
    /// �������� ���� �뷮 �����͸� ��Ÿ���� Ŭ����
    /// </summary>
    [Serializable]
    public class InstcapaData
    {
        #region Capacity Data
        [Tooltip("���� ���� �뷮 (MW)")]
        public double gelecInstcapa;

        [Tooltip("�� ���� �뷮 (Gcal/h)")]
        public double heatInstcapa;

        [Tooltip("���� ���� �뷮 (Gcal/h)")]
        public double heathInstcapa;
        #endregion

        /// <summary>
        /// �� ���� �뷮�� ����մϴ�.
        /// </summary>
        /// <returns>�� ���� �뷮 (MW)</returns>
        public double CalculateTotalCapacity()
        {
            return gelecInstcapa + heatInstcapa + heathInstcapa;
        }

        /// <summary>
        /// ������ ���� ���� ������ ����մϴ�.
        /// </summary>
        /// <returns>���� ���� ���� (�����)</returns>
        public double CalculateElectricityRatio()
        {
            double total = CalculateTotalCapacity();
            if (total == 0) return 0;
            return (gelecInstcapa / total) * 100;
        }
    }
}