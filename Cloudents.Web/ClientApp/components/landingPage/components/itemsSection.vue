<template>
    <div class="itemsSection">
        <h1 class="is-title">Students are Viewing</h1>
        <div class="itemsCarousel">
            <sbCarousel>
                    <div v-for="(item, index) in items" :key="index" class="itemCarouselCard"> 
                    <img class="itemCarouselImg" :src="getImg(item.img)" alt="">
                    <div class="item-cont pa-2">
                        <h1 class="item-title">{{item.name}}</h1>
                        <div class="item-course"><b>Course:</b> {{item.course}}</div>
                        <div class="item-university"><b>University:</b> {{item.university}}</div>
                        <div class="item-user">
                            <img class="userImg-item" :src="getImg(item.userImg)">
                            <div class="user-info">
                                <div>{{item.userName}}</div>
                                <div>{{item.date}}</div>
                            </div>
                        </div>
                        <div class="itemCard-bottom">
                            <span class="item-purchases">{{item.purchases}} Purchases</span>
                            <span class="item-pts">{{item.pts}} Pts</span>
                        </div>
                    </div>

                </div>
            </sbCarousel>
        </div>
    </div>
</template>

<script>
import sbCarousel from '../../sbCarousel/sbCarousel.vue';

export default {
    components:{
        sbCarousel
    },
    data() {
        return {
            items:[
                {   
                    img: './staticCardImgs/d1.png',
                    name:'מבחן תשעה סמסטר ב מועד ב',
                    course: 'Statistics',
                    university: 'Ariel university shomron',
                    userName:'By Eidan kingsly',
                    userImg: './staticCardImgs/u1.png',
                    date:'Jun 30 2019',
                    purchases: '75',
                    pts: 750
                },
                {   
                    img: './staticCardImgs/d2.png',
                    name:'Step by step algebra course',
                    course: 'Statistics',
                    university: 'Ariel university shomron',
                    userName:'By Dror Rozenfeld',
                    userImg: './staticCardImgs/u2.png',
                    date:'Jun 30 2019',
                    purchases: '1,000',
                    pts: 750
                },
                {   
                    img: './staticCardImgs/d3.png',
                    name:'מבחן תשעה סמסטר ב מועד ב',
                    course: 'Statistics',
                    university: 'Ariel university shomron',
                    userName:'By Eidan kingsly',
                    userImg: './staticCardImgs/u3.png',
                    date:'Jun 30 2019',
                    purchases: 100,
                    pts: 750
                },
                {   
                    img: './staticCardImgs/d4.png',
                    name:'מבחן תשעה סמסטר ב מועד ב',
                    course: 'Statistics',
                    university: 'Ariel university shomron',
                    userName:'By Eidan kingsly',
                    userImg: './staticCardImgs/u4.png',
                    date:'Jun 30 2019',
                    purchases: 100,
                    pts: 750
                },
                {   
                    img: './staticCardImgs/d5.png',
                    name:'מבחן תשעה סמסטר ב מועד ב',
                    course: 'Statistics',
                    university: 'Ariel university shomron',
                    userName:'By Eidan kingsly',
                    userImg: './staticCardImgs/u5.png',
                    date:'Jun 30 2019',
                    purchases: 100,
                    pts: 750
                },
                {   
                    img: './staticCardImgs/d1.png',
                    name:'מבחן תשעה סמסטר ב מועד ב',
                    course: 'Statistics',
                    university: 'Ariel university shomron',
                    userName:'By Eidan kingsly',
                    userImg: './staticCardImgs/u1.png',
                    date:'Jun 30 2019',
                    purchases: '75',
                    pts: 750
                },
                {   
                    img: './staticCardImgs/d2.png',
                    name:'Step by step algebra course',
                    course: 'Statistics',
                    university: 'Ariel university shomron',
                    userName:'By Dror Rozenfeld',
                    userImg: './staticCardImgs/u2.png',
                    date:'Jun 30 2019',
                    purchases: '1,000',
                    pts: 750
                },
                {   
                    img: './staticCardImgs/d3.png',
                    name:'מבחן תשעה סמסטר ב מועד ב',
                    course: 'Statistics',
                    university: 'Ariel university shomron',
                    userName:'By Eidan kingsly',
                    userImg: './staticCardImgs/u3.png',
                    date:'Jun 30 2019',
                    purchases: 100,
                    pts: 750
                },
                {   
                    img: './staticCardImgs/d4.png',
                    name:'מבחן תשעה סמסטר ב מועד ב',
                    course: 'Statistics',
                    university: 'Ariel university shomron',
                    userName:'By Eidan kingsly',
                    userImg: './staticCardImgs/u4.png',
                    date:'Jun 30 2019',
                    purchases: 100,
                    pts: 750
                },
                {   
                    img: './staticCardImgs/d5.png',
                    name:'מבחן תשעה סמסטר ב מועד ב',
                    course: 'Statistics',
                    university: 'Ariel university shomron',
                    userName:'By Eidan kingsly',
                    userImg: './staticCardImgs/u5.png',
                    date:'Jun 30 2019',
                    purchases: 100,
                    pts: 750
                },
            ]
        }
    },
  methods: {
    getImg(path) {
      return require(`${path}`);
    }
  }

}
</script>

<style lang="less">
.itemsSection{
    margin-bottom: 80px;
    .is-title{
        font-size: 24px;
        font-weight: bold;
        font-stretch: normal;
        font-style: normal;
        line-height: normal;
        letter-spacing: normal;
        margin-bottom: 20px;
        color: #43425d;
    }
    .itemsCarousel{
        width: 100%;
        .itemCarouselCard{
            width: 242px;
            height: 340px;
            background: white;
            border-radius: 8px;
            border: solid 1px #c1c3d2;
            color: #43425d;
            .itemCarouselImg{
                border-bottom:  solid 1px #c1c3d2
            }
            .item-cont{
                .item-title{
                    font-size: 14px;
                    font-weight: bold;
                    font-stretch: normal;
                    font-style: normal;
                    line-height: normal;
                    letter-spacing: normal;
                }
                .item-course{
                    margin: 8px 0;
                    font-size: 12px;
                    font-weight: normal;
                    font-stretch: normal;
                    font-style: normal;
                    line-height: normal;
                    letter-spacing: normal;
                }
                .item-university{
                    font-size: 12px;
                    font-weight: normal;
                    font-stretch: normal;
                    font-style: normal;
                    line-height: normal;
                    letter-spacing: normal;
                }
                .item-user{
                    margin-top: 30px;
                    margin-bottom: 14px;
                    display: flex;
                    align-items: unset;
                    .userImg-item{
                        margin-right: 10px;
                    }
                    .user-info{
                        font-size: 12px;
                        font-weight: normal;
                        font-stretch: normal;
                        font-style: normal;
                        line-height: normal;
                        letter-spacing: normal;
                        color: #43425d;
                    }
                }
                .itemCard-bottom{
                    display: flex;
                    justify-content: space-between;
                    align-items: center;
                    .item-purchases{
                        font-size: 12px;
                        font-weight: 600;
                        font-stretch: normal;
                        font-style: normal;
                        line-height: normal;
                        letter-spacing: normal;
                        color: #43425d;
                    }
                    .item-pts{
                        font-size: 14px;
                        font-weight: bold;
                        font-stretch: normal;
                        font-style: normal;
                        line-height: normal;
                        letter-spacing: normal;
                    }

                }
            }
        }
    }

}

</style>