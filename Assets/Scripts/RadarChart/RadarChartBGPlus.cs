﻿/*
 * UGUIChartLib
 * Copyright © 2019 w199753. 
 * feedback:http://15384855139@163.com
*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace ChartLib
{

    [Serializable]
    public struct BGAttribute
    {
        public float lineCurde;
        public Color upBorderColor;
        public Color buttomBorderColor;

        public BGAttribute(float lineCurde, Color upBorderColor, Color buttomBorderColor)
        {
            this.lineCurde = lineCurde;
            this.upBorderColor = upBorderColor;
            this.buttomBorderColor = buttomBorderColor;
        }
    }

    public class RadarChartBGPlus : ChartBase
    {

        [SerializeField]
        public List<BGAttribute> bgInfoList = new List<BGAttribute>();

        [SerializeField]
        int parametersCount;//数据项个数

        [SerializeField]
        private ColorMode colorMode = ColorMode.Single;

        [SerializeField]
        int circleCount = 3;

        [SerializeField, Range(1f, 2f)]
        private float guideLineWidth = 1.5f;
        [SerializeField]
        private Color lineColor = Color.white;

        [SerializeField, Range(0.1f, 0.5f)]
        float lineCurde = 0.1f;//线条的粗度
        public float LineCurde
        {
            get { return lineCurde; }
            set { lineCurde = value; graphic.SetVerticesDirty(); }
        }

        [SerializeField, Range(10, 50)]
        private float lineDleta = 10;


        public override void ModifyMesh(VertexHelper vh)
        {
            ModifyVertices(vh);
        }


        static int tmpCount = 0;
        private void ModifyVertices(VertexHelper vh)
        {
            vh.Clear();

            float angle = 360f / parametersCount;

            //判断是否需要缓存
            if (tmpCount != parametersCount)
            {
                CacheUnit.CacheItemAsAngle(parametersCount, angle);
            }
            tmpCount = parametersCount;


            float innerX = (Kx - LineCurde * Kx), innerY = (Ky - LineCurde * Ky);
            float outerX = Kx, outerY = Ky;

            for (int z = 0; z < circleCount; z++)
            {
                float delta = lineDleta * z;
                for (int j = 0; j < parametersCount; j++)
                {
                    //设置颜色模式
                    int sequence = -1;
                    if (colorMode == ColorMode.Single)
                        sequence = z;
                    else
                        sequence = j;

                    float cos = CacheUnit.cacheCosList[j];
                    float sin = CacheUnit.cacheSinList[j];
                    float nextCos = CacheUnit.cacheCosList[j + 1];
                    float nextSin = CacheUnit.cacheSinList[j + 1];


                    //上边    顺序：左下角  左上角    右上角   右下角
                    drawAttribute.SetPosition(CacheUnit.SetVector(nextCos * (innerX - delta), nextSin * (innerY - delta)), CacheUnit.SetVector(nextCos * (outerX - delta), nextSin * (outerY - delta)),
                         CacheUnit.SetVector(cos * (outerX - delta), sin * (outerY - delta)), CacheUnit.SetVector(cos * (innerX - delta), sin * (innerY - delta)));
                    drawAttribute.SetColor(bgInfoList[sequence].upBorderColor, bgInfoList[sequence].buttomBorderColor,
                        bgInfoList[sequence].buttomBorderColor, bgInfoList[sequence].upBorderColor);
                    DrawSimpleQuad(vh, drawAttribute);

                }
            }

            //绘制辅助线
            for (int j = 0; j < parametersCount; j++)
            {
                float cos = CacheUnit.cacheCosList[j];
                float sin = CacheUnit.cacheSinList[j];

                float K = (sin * (Ky)) / (cos * (outerX));//计算斜率，增加线条宽度

                K = Mathf.Abs(K);

                if (K > 500f)//看做无限大  垂直
                {
                    drawAttribute.SetPosition(CacheUnit.SetVector(-guideLineWidth, guideLineWidth), CacheUnit.SetVector(cos * (outerX) - guideLineWidth, sin * (Ky) + guideLineWidth),
                        CacheUnit.SetVector(cos * (outerX) + guideLineWidth, sin * (outerY) - guideLineWidth), CacheUnit.SetVector(guideLineWidth, -guideLineWidth));
                    drawAttribute.SetColor(lineColor, lineColor, lineColor, lineColor);
                    DrawSimpleQuad(vh, drawAttribute);
                    continue;
                }
                drawAttribute.SetPosition(CacheUnit.SetVector(-guideLineWidth, guideLineWidth + K), CacheUnit.SetVector(cos * (outerX) + guideLineWidth, sin * (Ky) + guideLineWidth + K),
                    CacheUnit.SetVector(cos * (outerX) + guideLineWidth, sin * (outerY) - guideLineWidth - K), CacheUnit.SetVector(-guideLineWidth, -guideLineWidth - K));
                drawAttribute.SetColor(lineColor, lineColor, lineColor, lineColor);
                DrawSimpleQuad(vh, drawAttribute);

            }

        }

    }

}