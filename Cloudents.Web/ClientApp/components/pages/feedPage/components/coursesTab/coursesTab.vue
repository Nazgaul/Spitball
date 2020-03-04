<template>
    <v-flex class="information-box">
        <div class="information-box-mobile-wrap">
            <v-icon v-show="showHamburger" class="gamburger-icon" @click="setNavigationDrawerState()">sbf-menu</v-icon>
            <span class="information-box-text" :class="{'mobile': isMobile}">{{informationBlockText}}</span>
        </div>
    </v-flex>
</template>

<script>
import {mapActions, mapGetters} from 'vuex'
import {LanguageService} from "../../../../../services/language/languageService"

export default {
    name: "verticals-tabs",
    computed: {
      ...mapGetters(['getUserLoggedInStatus','accountUser']),
        isMobile(){
            return this.$vuetify.breakpoint.mdAndDown
        },
        informationBlockText(){
            if(!!this.$route.query && !!this.$route.query.Course){
                return this.$route.query.Course
            } else if(!!this.$route.query.term){
                return this.$route.query.term
            } else{
                return LanguageService.getValueByKey("schoolBlock_all_courses");
            }
        },
        showHamburger(){
          if(this.$vuetify.breakpoint.xsOnly){
            if(this.getUserLoggedInStatus && this.accountUser?.userType !== 'Parent'){
              return true
            }else{
              return false;
            }
          }else{
            return false;
          }
        }
    },
    methods: {
        ...mapActions(['toggleShowSchoolBlock']),
        setNavigationDrawerState(){
                this.toggleShowSchoolBlock()
        }
    }
}
</script>

<style lang="less">
@import '../../../../../styles/mixin.less';
.information-box{
  align-items: center;
  padding-bottom: 12px;
  @media (max-width: @screen-xs) {
    padding: 10px;
  }
  .gamburger-icon{
    font-size: 12px !important;
    line-height: 22px !important;
    color: rgba(67, 66, 93, 0.6) !important;
    margin-right: 5px !important;
  }
  .information-box-mobile-wrap{
    box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
    background-color: #fff;
    border-radius: 6px;
    height: 46px;
    align-items: center;
    line-height: 4;
    display: flex;
    padding-left: 14px;
    @media (max-width: @screen-xs) {
      box-shadow: none;
      border-radius: 8px;
      height: 40px;
      padding-left: 10px;
    }
  }
  .information-box-text{
    font-size: 14px;
    font-weight: bold;
    font-style: normal;
    font-stretch: normal;
    line-height: normal;
    letter-spacing: -0.2px;
    color: @global-purple;
    &.mobile{
      font-weight: 600;
      margin-left: 6px;
      width: 90%;
      text-overflow: ellipsis;
      white-space: nowrap;
      overflow: hidden;
      font-weight: 600;
      color: #4d4b69;
    }
  }
}
</style>