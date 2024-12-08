# Solar Power Plant Management System
태양광 발전소 통합 관제 및 모니터링 시스템

[![Video Label](http://img.youtube.com/vi/0-Z_JaoFdKU/0.jpg)](https://youtu.be/0-Z_JaoFdKU)

## 📌 프로젝트 소개
신재생에너지의 수요 증가에 따라 태양광 발전소의 효율적인 관리와 모니터링을 위한 통합 관제 시스템입니다. 전국 발전량 현황과 솔라시도 발전소를 디지털 트윈으로 구현하여 실시간 모니터링 및 관리가 가능합니다.

## 🛠 주요 기능
### 1. 전국 현황 대시보드
![대시보드](https://github.com/YoungKwang-Kim/SKKU_SolarPower_Project/assets/54823568/3d00d73a-aa71-4dcc-a2ed-33e58d523c78)
- 지역별 실시간 발전량 모니터링
- 날짜/시간별 발전량 데이터 조회
- XCharts를 활용한 데이터 시각화
- 지역별 발전량 순위 표시

### 2. 솔라시도 발전소 디지털 트윈
![솔라시도](https://github.com/YoungKwang-Kim/SKKU_SolarPower_Project/assets/54823568/5b0f0f20-7bf4-4a50-83bc-a99e53f5e420)
- 구역별 실시간 발전량 모니터링
- 드론 점검 시뮬레이션
- 적외선 카메라 시스템
- 이상 상황 자동 알림

### 3. 태양 추적 시스템
![태양고도](https://github.com/YoungKwang-Kim/SKKU_SolarPower_Project/assets/54823568/704ffac7-4127-4c4b-914f-aa6273944818)
- 실시간 태양 고도 추적
- 패널 각도 자동 조절
- 발전 효율 최적화

## 💻 기술 스택
- **Frontend**: Unity Engine
- **Backend**: Python, Firebase
- **Database**: Firebase Realtime Database
- **Others**: XCharts, Public API

## 🔍 주요 구현 사항
### 데이터 수집 및 처리
- REMS 웹크롤링을 통한 실시간 데이터 수집
- Firebase Realtime Database 연동
- 한국천문연구원 태양고도 정보 API 활용

### 시각화 및 모니터링
- XCharts를 활용한 그래프 구현
- 실시간 발전량 모니터링 시스템
- 3D 디지털 트윈 구현

### 드론 점검 시스템
![드론점검](https://github.com/YoungKwang-Kim/SKKU_SolarPower_Project/assets/54823568/d5ed5c4b-a576-4ef8-9b2e-080c6888ab17)
- 자동 순찰 시스템
- 실시간 모니터링
- 이상 징후 감지

## 📊 시스템 구조
```bash
├── Dashboard Scene
│   ├── 실시간 데이터 모니터링
│   ├── 지역별 발전량 순위
│   └── 데이터 시각화
└── Solaseado Scene
   ├── 디지털 트윈
   ├── 드론 점검 시스템
   └── 태양 추적 시스템
