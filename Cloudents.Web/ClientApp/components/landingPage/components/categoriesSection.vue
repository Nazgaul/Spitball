<template>
    <div class="categoriesSection">
        <h1 class="cs-title">Popular Categories</h1>
        <div class="categories-top" >
            <div v-for="(card, index) in categoriesCards" :key="index" class="categories-card" :style="{'backgroundImage': `url(${getImg(card.img)}`}">
                <span class="card-title">{{card.name}}</span>    
            </div>
        </div>
        <div class="categories-bottom">
            <div class="categories-banner">
                <span class="banner-title">The study tools you need to succeed anytime anywhere</span>
                <span class="banner-content"><compSVG class="banner-svg"/> Access thousands of course specific study documents</span>
                <span class="banner-content"><pplSVG class="banner-svg"/> Find expert teachers ready to take you to the next level</span>
                <span class="banner-content"><vidSVG class="banner-svg"/> Choose online courses to learn on your schedule</span>
            </div>
            <div class="categories-carousel">
                <sbCarousel :slideStep="3">
                    <div class="carousel-card" v-for="(item, index) in carouselCards" :key="index" :style="{'backgroundImage': `url(${getImg(item.img)}`}">
                        <span class="card-title">{{item.name}}</span> 
                    </div>
                </sbCarousel>
            </div>
        </div>
    </div>
</template>

<script>
import compSVG from './staticCardImgs/com.svg'
import pplSVG from './staticCardImgs/ppl.svg'
import vidSVG from './staticCardImgs/vid.svg'
import sbCarousel from '../../sbCarousel/sbCarousel.vue';
export default {
    components:{
        compSVG,
        pplSVG,
        vidSVG,
        sbCarousel
    },
    data() {
        return {
            categoriesCards: [
                {
                    name:'Algebra',
                    img: './staticCardImgs/alg.png'
                },
                {
                    name:'Computer Sciences',
                    img: './staticCardImgs/computer-sciences.png'
                },
                {
                    name:'Economics',
                    img: './staticCardImgs/economics.png'
                },
                {
                    name:'Statistics',
                    img: './staticCardImgs/statistics.png'
                },
                {
                    name:'Psychology',
                    img: './staticCardImgs/psychology.png'
                }
            ],
            carouselCards:[
                {
                    name: 'Math 5 levels',
                    img: './staticCardImgs/mathC.png',
                },
                {   name: 'English',
                    img: './staticCardImgs/englishC.png'
                },
                {   name: 'Statistics',
                    img: './staticCardImgs/statisticsC.png'
                },
                {
                    name: 'Math 5 levels',
                    img: './staticCardImgs/mathC.png',
                },
                {   name: 'English',
                    img: './staticCardImgs/englishC.png'
                },
                {   name: 'Statistics',
                    img: './staticCardImgs/statisticsC.png'
                }
            ]
        }
    },
    methods: {
        getImg(path){
            return require(`${path}`)
        }
    },
}
</script>

<style lang="less">
.categoriesSection{
        margin-top: 50px;
        padding-bottom: 74px; // // // // // // sdd
        .cs-title{
            font-size: 24px;
            font-weight: bold;
            font-stretch: normal;
            font-style: normal;
            line-height: normal;
            letter-spacing: normal;
            color: #43425d;
        }
        .categories-top{
            margin-top: 18px;
            margin-bottom: 80px;
            display: flex;
            justify-content: space-between;
            color: white;
            .categories-card{
                position: relative;
                width: 242px;
                height: 136px;
                background-repeat: no-repeat;
                background-size: cover;
                // margin-right: 36px;
                :last-child{
                    margin-right: 0;
                }
                .card-title{
                    position: absolute;
                    left: 14px;
                    top: 8px;
                }
            }
        }
        .categories-bottom{ 
            display: flex;
            justify-content: space-between;
            height: 254px;
            .categories-banner{
                width: 519px;
                background-color: #f9f9fa;
                padding: 12px 18px;
                .banner-title{
                    display: block;
                    font-size: 26px;
                    font-weight: bold;
                    color: #43425d;
                    margin-bottom: 22px;
                }
                .banner-content{
                    display: inline-flex;
                    font-size: 14px;
                    color: #43425d;
                    align-items: center;
                    :last-child{
                        margin-bottom: 0;
                    }
                    .banner-svg{
                        
                    }
                }
            }
            .categories-carousel{
                width: calc(~"100% - 564px");
                .carousel-card{
                    width: 242px;
                    position: relative;
                    height: 256px;
                    background-repeat: no-repeat;
                    background-size: cover;
                    color: white;
                    :last-child{
                        margin-right: 0;
                    }
                    .card-title{
                        position: absolute;
                        left: 14px;
                        top: 8px;
                    }
                }
            }
        }
    }
</style>