<template>
    <div class="testimonialsSection">
        <h1 class="tss-subtitle">Testimonials</h1>
        <h1 class="tss-title">What our students say </h1>
        <div class="tss-dvider"/>
        <div class="testimonialsCont">
            <div class="testimonialCard" :class="{'cardMargin':index==1}" v-for="(item, index) in testimonials" :key="index">
                <img class="card-t" src="./staticCardImgs/cardT.png" alt="">
                <div class="testimonialCard-top">
                    <img class="quote-card" src="./staticCardImgs/quote.png" alt="">
                    <p class="testimonialCard-txt">{{item.txt}}</p>
                    <h1 class="t-name">{{item.userName}}</h1>
                </div>
                <div class="tutorInfoT">
                    <img class="tutorImg-t" :src="getImg(item.userImg)">
                    <div class="tutorName-t">{{item.tutorName}}</div>
                </div>
            </div>
        </div>

    </div>
</template>

<script>
export default {
    data() {
        return {
            testimonials:[
                {
                    txt:`מצליח להעביר את החומר בצורה מדוייקת ועניינית כולל תרגול של שלבי פיתרון . מסביר בדרך שאפשר להבין וסובלני מאוד. לא יפסיק להסביר עד שתגיד שהבנת ב100 אחוזים.`,
                    userName:'בת אל',
                    userImg: './staticCardImgs/ut1.png',
                    tutorName:'מורה פרטי אריאל פיינזילברג',
                },
                {
                    txt:`היה מעולה!
איזה כיף זה בלי לצאת מהבית בלי לחשוד ללכת לבתים של מורים שהכרת באינטרנט. מומלץ בחום`,
                    userName:'מורן ג',
                    userImg: './staticCardImgs/ut2.png',
                    tutorName:'מורה פרטי שלומי לוי',
                },
                {
                    txt:'בהחלט השגתי את ואפילו יותר את הציון שרציתי ציון 86 תודה לכם על עזרתכם הרבה במיוחד שהמלצת לי על טל ממש מעולה. ',
                    userName:'סיגלית ש',
                    userImg: './staticCardImgs/ut3.png',
                    tutorName:'מורה פרטי טל שהם',
                }
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
    .testimonialsSection{
        width: 100%;
        height: 620px;
        background-color: #f9f9fa;
        .tss-subtitle{
            padding-top: 34px;
            font-size: 16px;
            font-weight: normal;
            font-stretch: normal;
            font-style: normal;
            line-height: normal;
            letter-spacing: normal;
            text-align: center;
            color: #43425d;
        }
        .tss-title{
            font-size: 30px;
            font-weight: bold;
            font-stretch: normal;
            font-style: normal;
            line-height: normal;
            letter-spacing: normal;
            text-align: center;
            color: #43425d;
        }
        .tss-dvider{
            margin: 0 auto;
            margin-top: 16px;
            margin-bottom: 58px;
            width: 111px;
            height: 6px;
            background-color: #4c59ff;
        }
        .testimonialsCont{
            display: flex;
            justify-content: center;
            .testimonialCard{
                &.cardMargin{
                    margin: 0 84px;
                }
                padding: 0 20px;
                position: relative;
                width: 285px;
                height: 375px;
                border-radius: 8px;
                box-shadow: 0 2px 24px 0 rgba(0, 0, 0, 0.31);
                background-color: #ffffff;
                display: flex;
                flex-direction: column;
                justify-content: space-between;
                .card-t{
                    position: absolute;
                    top: 0;
                    right: -1px;
                }
                .testimonialCard-top{
                    display: flex;
                    flex-direction: column;
                    align-items: center;
                    .quote-card{
                        margin-top: 34px;
                    }
                    .testimonialCard-txt{
                        margin: 0;
                        padding: 0;
                        margin-top: 12px;
                        font-size: 16px;
                        font-weight: normal;
                        font-stretch: normal;
                        font-style: normal;
                        line-height: 1.75;
                        letter-spacing: normal;
                        text-align: center;
                        color: #43425d;
                    }

                    .t-name{
                        font-size: 16px;
                        font-weight: normal;
                        font-stretch: normal;
                        font-style: normal;
                        line-height: 2.51;
                        letter-spacing: normal;
                        color: #848bbc;
                    }
                }
                .tutorInfoT{
                    display: flex;
                    flex-direction: column;
                    align-items: center;
                    .tutorImg-t{
                        margin-bottom: 12px;
                    }
                    .tutorName-t{
                        font-size: 16px;
                        font-weight: normal;
                        font-stretch: normal;
                        font-style: normal;
                        line-height: 1.43;
                        letter-spacing: normal;
                        color: #4c59ff;
                        margin-bottom: 34px;
                    }
                }
            }
        }
    }
</style>