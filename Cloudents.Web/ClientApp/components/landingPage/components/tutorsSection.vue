<template>
    <div class="tutorsSections">
        <h1 class="ts-title">Popular Tutors</h1>
        <h2 class="ts-subtitle">Find the perfect  tutor and chat free of charge</h2>
        <div class="tutorsCarousel">
            <sbCarousel>
                <div v-for="(item, index) in tutors" :key="index" class="tutorCarouselCard ">
                    <img class="tutorCarouselImg" :src="getImg(item.img)" alt="">
                    <div class="ts-content pa-2">
                        <h1 class="tutor-name">{{item.name}}</h1>
                        <h2 class="tutor-university">{{item.university}}</h2>
                        <p class="tutor-bio my-2">{{item.bio}}</p>
                        <v-btn depressed color="#4c59ff" class="tutor-btn">Contact Doron</v-btn>
                        <div class="ts-bottom">
                            <div class="rank"></div>
                            <div class="ts-price"><span class="price-mark">{{item.price}}</span>/hour</div>
                        </div>
                    </div>
                </div>

            </sbCarousel>
        </div>
    </div>
</template>

<script>
import sbCarousel from '../../sbCarousel/sbCarousel.vue'
export default {
  components: {
    sbCarousel
  },
  data() {
    return {
      tutors: [
        {
          img: "./staticCardImgs/1.png",
          name: "Tutor Ariel fineslezibger",
          university: "University of Haifa",
          bio:
            "Iam a PhD candidate at Cambridge pursuing a PhD in International H",
          price: "₪55"
        },
        {
          img: "./staticCardImgs/2.png",
          name: "Tutor Moran mizrachi",
          university: "University of Ben gurion",
          bio:
            "Iam a PhD candidate at Cambridge pursuing a PhD in International H",
          price: "₪120"
        },
        {
          img: "./staticCardImgs/3.png",
          name: "Tutor Doron melamed",
          university: "Tel Aviv University",
          bio:
            "Iam a PhD candidate at Cambridge pursuing a PhD in International H",
          price: "₪75"
        },
        {
          img: "./staticCardImgs/4.png",
          name: "Tutor Shahar elmaliah",
          university: "Tel Aviv University",
          bio:
            "Iam a PhD candidate at Cambridge pursuing a PhD in International H",
          price: "₪55"
        },
        {
          img: "./staticCardImgs/user.jpg",
          name: "Tutor Shahar elmaliah",
          university: "Tel Aviv University",
          bio:
            "Iam a PhD candidate at Cambridge pursuing a PhD in International H",
          price: "₪55"
        },
        {
          img: "./staticCardImgs/1.png",
          name: "Tutor Doron melamed",
          university: "Tel Aviv University",
          bio:
            "Iam a PhD candidate at Cambridge pursuing a PhD in International H",
          price: "₪75"
        },
        {
          img: "./staticCardImgs/2.png",
          name: "Tutor Shahar elmaliah",
          university: "Tel Aviv University",
          bio:
            "Iam a PhD candidate at Cambridge pursuing a PhD in International H",
          price: "₪55"
        },
        {
          img: "./staticCardImgs/3.png",
          name: "Tutor Shahar elmaliah",
          university: "Tel Aviv University",
          bio:
            "Iam a PhD candidate at Cambridge pursuing a PhD in International H",
          price: "₪55"
        },
        {
          img: "./staticCardImgs/4.png",
          name: "Tutor Doron melamed",
          university: "Tel Aviv University",
          bio:
            "Iam a PhD candidate at Cambridge pursuing a PhD in International H",
          price: "₪75"
        },
        {
          img: "./staticCardImgs/2.png",
          name: "Tutor Shahar elmaliah",
          university: "Tel Aviv University",
          bio:
            "Iam a PhD candidate at Cambridge pursuing a PhD in International H",
          price: "₪55"
        },
        {
          img: "./staticCardImgs/user.jpg",
          name: "Tutor Shahar elmaliah",
          university: "Tel Aviv University",
          bio:
            "Iam a PhD candidate at Cambridge pursuing a PhD in International H",
          price: "₪55"
        },
        {
          img: "./staticCardImgs/1.png",
          name: "Tutor Doron melamed",
          university: "Tel Aviv University",
          bio:
            "Iam a PhD candidate at Cambridge pursuing a PhD in International H",
          price: "₪75"
        },
        {
          img: "./staticCardImgs/2.png",
          name: "Tutor Shahar elmaliah",
          university: "Tel Aviv University",
          bio:
            "Iam a PhD candidate at Cambridge pursuing a PhD in International H",
          price: "₪55"
        },
        {
          img: "./staticCardImgs/3.png",
          name: "Tutor Shahar elmaliah",
          university: "Tel Aviv University",
          bio:
            "Iam a PhD candidate at Cambridge pursuing a PhD in International H",
          price: "₪55"
        }
      ]
    };
  },
  methods: {
    getImg(path) {
      return require(`${path}`);
    }
  }
};
</script>

<style lang="less">
.tutorsSections {
  margin-bottom: 80px;
  .ts-title {
    font-size: 24px;
    font-weight: bold;
    font-stretch: normal;
    font-style: normal;
    line-height: normal;
    letter-spacing: normal;
    color: #43425d;
  }
  .ts-subtitle {
    margin: 10px 0;
    font-size: 16px;
    font-weight: normal;
    font-stretch: normal;
    font-style: normal;
    line-height: normal;
    letter-spacing: normal;
    color: #43425d;
  }
  .tutorsCarousel {
    width: 100%;
    .tutorCarouselCard {
      width: 242px;
      height: 340px;
      background: white;
      border-radius: 8px;
      border: solid 1px #c1c3d2;
      color: #43425d;
      .tutorCarouselImg {
        border-top-left-radius: 8px;
        border-top-right-radius: 8px;
      }
      .ts-content {
        .tutor-name {
          font-size: 14px;
          font-weight: bold;
          font-stretch: normal;
          font-style: normal;
          line-height: normal;
          letter-spacing: normal;
        }
        .tutor-university {
          font-size: 14px;
          font-weight: normal;
          font-stretch: normal;
          font-style: normal;
          line-height: normal;
          letter-spacing: normal;
        }
        .tutor-bio {
          margin: 0;
          padding: 0;
          font-size: 13px;
          font-weight: normal;
          font-stretch: normal;
          font-style: normal;
          line-height: 1.54;
          letter-spacing: normal;
        }
        .tutor-btn {
          border-radius: 8px;
          color: white;
          font-size: 14px;
          font-weight: 600;
          text-transform: capitalize !important;
          height: 34px !important;
          min-width: 100%;
          margin: 0;
          margin-bottom: 16px;
        }
        .ts-bottom {
          display: flex;
          justify-content: space-between;
          .ts-price {
            font-size: 12px;
            font-weight: normal;
            .price-mark {
              font-weight: bold;
              font-size: 16px;
            }
          }
        }
      }
    }
  }
}
</style>