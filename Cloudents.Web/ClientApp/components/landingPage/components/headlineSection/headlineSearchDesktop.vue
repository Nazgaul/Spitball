<template>
    <div class="headlineSearchDesktop">
      <div class="headlineTitlesDesktop">
        <h1 class="hd-titleDesktop" v-language:inner="'homePage_hd_title'"/>
        <h2 :class="['hd-subtitleDesktop','my-4']" v-language:inner="'homePage_hd_subtitle'"/>
        <div class="hd-searchDesktop">
          <div class="search-input-cont">
            <v-text-field
              class="search-input-hp"
              v-model="search"
              @keyup.enter="searchQuery"
              solo
              prepend-inner-icon="sbf-search"
              :placeholder="phSearch"
              autocomplete="off"
              hide-details
              type="search"
            ></v-text-field>
          </div>
          <div @click="searchQuery" class="search-btn" v-language:inner="'homePage_hd_search'" />
        </div>
      </div>
      <div class="headline-img">
        <div class="handImg" :style="{'backgroundImage': `url(${handImg}`}"/>
      </div>
    </div>
</template>

<script>
import { LanguageService } from "../../../../services/language/languageService.js";
import { mapGetters } from 'vuex';

export default {
  data() {
    return {
      search: "",
      phSearch: LanguageService.getValueByKey("homePage_hd_search_ph")
    };
  },
  methods: {
    searchQuery() {
      if (this.search) {
        this.$router.push({ path: "/feed", query: { term: this.search } });
      }
    }
  },
  computed: {
    ...mapGetters(['isFrymo']),
    handImg(){
      if(this.isFrymo){
        return require('../../images/Hand_Frymo.png')
      }else{
        if(global.isRtl){
          return require('../../images/Hand_he.png')
        }else{
          return require('../../images/Hand_en.png')
        }
      }
    }
  }
};
</script>

<style lang="less" >
@import "../../../../styles/mixin.less";

.headlineSearchDesktop {
    .responsiveLandingPage(1354px, 0px);
    width: 100%;

    color: #ffffff;
    font-stretch: normal;
    font-style: normal;
    letter-spacing: normal;
    height: 100%;
    display: flex;
    align-items: center;
    margin: 0 auto;
    .headline-img {
      width: 50%;
      height: 100%;
      .handImg {
        width: 105%;
        height: 100%;
        background-repeat: no-repeat;
        background-size: cover;
        background-position-x: left;
      }
    }
    .headlineTitlesDesktop {
        margin-left: 50px;
        z-index: 1;
        padding-bottom: 70px;
      width: 50%;
      .hd-titleDesktop {
        font-size: 36px;
        font-weight: bold;
      }
      .hd-subtitleDesktop {
        font-size: 16px;
        font-weight: normal;
        padding-right: 60px;
      }
      .hd-searchDesktop {
        overflow: hidden;
        width: 470px;
        height: 50px;
        border-radius: 12px;
        display: flex;
        .search-input-cont {
          background: white !important;

          flex: 2;
          height: 100%;
          .search-input-hp {
            height: 100%;
            .v-input__control {
              height: 100%;
              min-height: initial;
              .v-input__slot {
                background: none;
                box-shadow: none;
                height: 100%;
                .v-input__icon {
                  i {
                    color: #c3c3d0;
                  }
                }
                .v-text-field__slot {

                  font-size: 18px;
                  font-weight: normal;
                  font-stretch: normal;
                  font-style: normal;
                  line-height: normal;
                  letter-spacing: normal;
                  color: #a1a3b0;
                }
              }
            }
          }
        }
        .search-btn {
          cursor: pointer;
          font-size: 20px;
          display: flex;
          align-items: center;
          justify-content: center;
          height: 100%;
          background-color: #4c59ff;
          padding: 0 20px;
        }
      }
    }
  }
</style>