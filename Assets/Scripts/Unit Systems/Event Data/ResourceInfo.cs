using UnityEngine;

namespace StatSystem
{
    public struct ResourceInfo
    {
        public ResourceType resourceType;       // 자원 타입
        public float currrentResource;          // 현재 자원량
        public float maxResource;               // 최대 자원량

        public ResourceInfo(ResourceType resourceType, float currrentResource, float maxResource)
        {
            this.resourceType = resourceType;
            this.currrentResource = currrentResource;
            this.maxResource = maxResource;
        }

        /// <summary>
        /// 최대 자원 대 현재 자원의 비율을 반환합니다.
        /// </summary>
        /// <returns></returns>
        public float ResourceRatio => maxResource > 0 ? (float)currrentResource / maxResource : 0;            // 최대 마나가 0 이라하면 비율도 0으로 반환
    }
}
