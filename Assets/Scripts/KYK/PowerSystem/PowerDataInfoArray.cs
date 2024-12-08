using System;
using System.Collections.Generic;
using UnityEngine;

namespace PowerPlant.Data
{
    /// <summary>
    /// 발전소의 전체 전력 데이터를 담는 컨테이너 클래스
    /// </summary>
    [Serializable]
    public class PowerDataInfoArray
    {
        public List<PowerData> powerDataInfo;
    }

    /// <summary>
    /// 개별 발전소의 전력 생산 및 사용 데이터를 나타내는 클래스
    /// </summary>
    [Serializable]
    public class PowerData
    {
        #region Production Data
        [Tooltip("금일 발전량 (MWh)")]
        public double dayGelec;

        [Tooltip("누적 발전량 (MWh)")]
        public double accumGelec;
        #endregion

        #region Consumption Data
        [Tooltip("금일 사용량 (MWh)")]
        public double dayPrdct;

        [Tooltip("누적 사용량 (MWh)")]
        public double cntuAccumPowerPrdct;
        #endregion

        #region Environmental Data
        [Tooltip("일일 CO2 저감량 (톤)")]
        public int co2;

        [Tooltip("시간당 CO2 저감량 (톤/시)")]
        public double hco2;
        #endregion

        /// <summary>
        /// 발전 효율을 계산합니다.
        /// </summary>
        /// <returns>발전 효율 (백분율)</returns>
        public double CalculateEfficiency()
        {
            if (dayPrdct == 0) return 0;
            return (dayGelec / dayPrdct) * 100;
        }

        /// <summary>
        /// CO2 저감 효율을 계산합니다.
        /// </summary>
        /// <returns>시간당 평균 CO2 저감량</returns>
        public double CalculateAverageCO2Reduction()
        {
            const int HOURS_PER_DAY = 24;
            return co2 / (double)HOURS_PER_DAY;
        }
    }

    /// <summary>
    /// 발전소의 설비 용량 데이터를 나타내는 클래스
    /// </summary>
    [Serializable]
    public class InstcapaData
    {
        #region Capacity Data
        [Tooltip("전기 설비 용량 (MW)")]
        public double gelecInstcapa;

        [Tooltip("열 설비 용량 (Gcal/h)")]
        public double heatInstcapa;

        [Tooltip("난방 설비 용량 (Gcal/h)")]
        public double heathInstcapa;
        #endregion

        /// <summary>
        /// 총 설비 용량을 계산합니다.
        /// </summary>
        /// <returns>총 설비 용량 (MW)</returns>
        public double CalculateTotalCapacity()
        {
            return gelecInstcapa + heatInstcapa + heathInstcapa;
        }

        /// <summary>
        /// 설비의 전기 생산 비율을 계산합니다.
        /// </summary>
        /// <returns>전기 생산 비율 (백분율)</returns>
        public double CalculateElectricityRatio()
        {
            double total = CalculateTotalCapacity();
            if (total == 0) return 0;
            return (gelecInstcapa / total) * 100;
        }
    }
}