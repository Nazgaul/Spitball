<template>
<div class="symbol-container" style="display: flex;">
<ul>
    <li @click="injectToTextArea(`\\log_{a}{b}`)"><log-svg></log-svg></li>
    <li @click="injectToTextArea(`\\sqrt[n]{ab}`)"><sqrtn-svg></sqrtn-svg></li>
    <li @click="injectToTextArea(`\\sqrt{ab}`)"><sqrt-svg></sqrt-svg></li>
    <li @click="injectToTextArea(`\\frac{\\text{d}x}{\\text{d}y}`)"><frac-text-svg></frac-text-svg></li>
    <li @click="injectToTextArea(`\\frac{ab}{cd}`)"><frac-svg></frac-svg></li>
    <li @click="injectToTextArea(`\\lim_{a \\rightarrow b}`)"><lim-right-svg></lim-right-svg></li>
    <li @click="injectToTextArea(`\\int_{a}^{b}`)"><int-svg></int-svg></li>
    <li @click="injectToTextArea(`\\prod_a^b`)"><prod-svg></prod-svg></li>
</ul>
    <ul>
        <span class="li-wrap">
            <li @click="injectToTextArea(`\\left(`)"><left-curl-svg></left-curl-svg></li>
            <li @click="injectToTextArea(`\\right)`)"><right-curl-svg></right-curl-svg></li>
        </span>
        <span class="li-wrap">
            <li @click="injectToTextArea(`\\left[`)"><left-square-svg></left-square-svg></li>
            <li @click="injectToTextArea(`\\right]`)"><right-square-svg></right-square-svg></li>
        </span>
        <span class="li-wrap">
            <li @click="injectToTextArea(`\\left\\{`)"><left-curly-svg></left-curly-svg></li>
            <li @click="injectToTextArea(`\\right\\}`)"><right-curly-svg></right-curly-svg></li>
        </span>
        <li @click="injectToTextArea(`\\leftarrow`)"><s-Left-Arrow-Svg></s-Left-Arrow-Svg></li>
        <li @click="injectToTextArea(`\\Leftarrow`)"><l-Left-Arrow-Svg></l-Left-Arrow-Svg></li>
        <li @click="injectToTextArea(`\\rightarrow`)"><s-Right-Arrow-Svg></s-Right-Arrow-Svg></li>
        <li @click="injectToTextArea(`\\Rightarrow`)"><l-Right-Arrow-Svg></l-Right-Arrow-Svg></li>
        <li @click="injectToTextArea(`\\leftrightarrow`)"><left-Right-Arrow></left-Right-Arrow></li>
    </ul>
    <ul>
        <li @click="injectToTextArea(`\\alpha`)"><alpha-Svg></alpha-Svg></li>
        <li @click="injectToTextArea(`\\beta`)"><beta-Svg></beta-Svg></li>
        <li @click="injectToTextArea(`\\gamma`)"><gamma-Svg></gamma-Svg></li>
        <li @click="injectToTextArea(`\\delta`)"><delta-Svg></delta-Svg></li>
        <li @click="injectToTextArea(`\\epsilon`)"><epsilon-Svg></epsilon-Svg></li>
        <li @click="injectToTextArea(`\\zeta`)"><zeta-Svg></zeta-Svg></li>
        <li @click="injectToTextArea(`\\eta`)"><eta-Svg></eta-Svg></li>
        <li @click="injectToTextArea(`\\theta`)"><theta-Svg></theta-Svg></li>
    </ul>
</div>
</template>

<script>
import sqrtSvg from './svg/fn/sqrt.svg';
import sqrtnSvg from './svg/fn/sqrtn.svg';
import logSvg from './svg/fn/log.svg';
import fracTextSvg from './svg/fn/fracText.svg';
import fracSvg from './svg/fn/frac.svg';
import limRightSvg from './svg/fn/limRight.svg';
import intSvg from './svg/fn/int.svg';
import prodSvg from './svg/fn/prod.svg';

import leftCurlSvg from './svg/operators/leftCurl.svg';
import rightCurlSvg from './svg/operators/rightCurl.svg';
import leftSquareSvg from './svg/operators/leftSquare.svg';
import rightSquareSvg from './svg/operators/rightSquare.svg';
import leftCurlySvg from './svg/operators/leftCurly.svg';
import rightCurlySvg from './svg/operators/rightCurly.svg';
import sLeftArrowSvg from './svg/operators/sLeftArrow.svg';
import sRightArrowSvg from './svg/operators/sRightArrow.svg';
import lLeftArrowSvg from './svg/operators/lLeftArrow.svg';
import lRightArrowSvg from './svg/operators/lRightArrow.svg';
import leftRightArrow from './svg/operators/leftRightArrow.svg';

import alphaSvg from './svg/greek/alpha.svg';
import betaSvg from './svg/greek/beta.svg';
import gammaSvg from './svg/greek/gamma.svg';
import deltaSvg from './svg/greek/delta.svg';
import epsilonSvg from './svg/greek/epsilon.svg';
import zetaSvg from './svg/greek/zeta.svg';
import etaSvg from './svg/greek/eta.svg';
import thetaSvg from './svg/greek/theta.svg';
export default {
    components:{
        sqrtSvg,
        sqrtnSvg,
        logSvg,
        fracTextSvg,
        fracSvg,
        limRightSvg,
        intSvg,
        prodSvg,
        leftCurlSvg,
        rightCurlSvg,
        leftSquareSvg,
        rightSquareSvg,
        leftCurlySvg,
        rightCurlySvg,
        sLeftArrowSvg,
        sRightArrowSvg,
        lLeftArrowSvg,
        lRightArrowSvg,
        leftRightArrow,
        alphaSvg,
        betaSvg,
        gammaSvg,
        deltaSvg,
        epsilonSvg,
        zetaSvg,
        etaSvg,
        thetaSvg,
    },
    props:{
        injectToTextArea:{
            type:Function
        }
    }
}
</script>

<style lang="less">
.symbol-container{
    ul{
        display: flex;
        flex-direction: column;
        justify-content: space-between;
        padding-right:15px;
        padding-left: 5px;
        .li-wrap{
            display: flex;
            justify-content: space-between;
        }
        li{
            border: 1px solid #c4c4ca;
            border-radius: 4px;
            padding: 5px;
            margin: 2px;
            text-align: center;
            &:hover{
                background: #f0f0f7;
                cursor: pointer;
            }
        }
    }
}
</style>
