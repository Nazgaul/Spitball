<template>
    <v-list class="menuList" dense>
      
        <div class="menuListTopNotLogged pt-8 text-center" v-if="!isLoggedIn && isMobile">
            <div class="menuListTopLogo"><logoComponent/></div>
            <div class="menuListTopText mt-3" v-t="'menuList_txt_out'"></div>
            <div class="menuListTopBtns">
                <v-btn 
                  @click="openLoginDialog" 
                  class="menuListTopBtnsIn font-weight-bold mr-4" 
                  v-t="'menuList_Login'"
                  color="white" 
                  width="120"
                  height="38"
                  rounded 
                  depressed 
                >
                </v-btn>
                <v-btn 
                  @click="openRegisterTypeDialog"
                  class="menuListTopBtnsUp font-weight-bold white--text"
                  v-t="'menuList_Sign_up'"
                  color="#4c59ff"
                  width="120"
                  height="38"
                  rounded
                  depressed
                >
                </v-btn>
            </div>
        </div>

        <template v-if="isLoggedIn">
            <div class="menuListTopPosition"></div>

            <div class="userMenu_top text-center py-5">
                <userAvatar size="80" :userImageUrl="user.image" :user-name="user.name" :user-id="user.id"/>
                <div class="uM_top_txts">
                    <h1 class="uM_title">{{$t('menuList_greets', { '0': user.name })}}</h1>
                    <h2 class="uM_subtitle">{{$t('menuList_balance', { '0': userBalance(user.balance)})}}</h2>
                </div>
            </div>
        </template>

        <div class="userMenu_actionsList">
            <template v-if="showChangeLanguage">
                <v-list-item v-for="singleLang in languageChoisesAval" :key="singleLang.name" @click="changeLanguage(singleLang.id)" sel="menu_row">
                    <v-list-item-action>
                        <v-icon size="18" color="#69687d">{{singleLang.icon}}</v-icon>
                    </v-list-item-action>
                    <v-list-item-content>
                        <v-list-item-title class="subheading userMenu_titles">{{singleLang.title}}</v-list-item-title>
                    </v-list-item-content>
                </v-list-item>
            </template>

            <component
              :is="menuListUserType"
              :menuListNotLogged="menuListNotLogged"
              :menuListTeacher="menuListTeacher"
              :menuListStudent="menuListStudent"
            ></component>

            <!-- Logout -->
            <v-list-item @click="logout" v-if="isLoggedIn" sel="menu_row">
                <v-list-item-action>
                  <v-icon size="18" color="#69687d">sbf-logout</v-icon>
                </v-list-item-action>
                <v-list-item-content>
                  <v-list-item-title class="subheading userMenu_titles" v-t="'menuList_logout'"></v-list-item-title>
                </v-list-item-content>
            </v-list-item>
        </div>
    </v-list>
</template>

<script>
import { mapGetters, mapActions } from "vuex";

import languagesLocales from "../../../../services/language/localeLanguage";
import { LanguageChange } from "../../../../services/language/languageService";
import satelliteService from '../../../../services/satelliteService';
import intercomSettings from '../../../../services/intercomService';

import * as routeNames from '../../../../routes/routeNames'

import logoComponent from '../../../app/logo/logo.vue';
import menuListStudent from './menuListStudent.vue';
import menuListTeacher from './menuListTeacher.vue';
import menuListNotLogged from './menuListNotLogged.vue';

export default {
  components: {
    logoComponent,
    menuListStudent,
    menuListTeacher,
    menuListNotLogged
  },
  data() {
    return {
      languagesLocales,
      languageChoisesAval: [],
      menuListComponent: {
        default: 'menuListNotLogged',
        teacher: 'menuListTeacher',
        student: 'menuListStudent',
      },
      menuListNotLogged: [
        { title: 'header_find_tutors', icon: 'sbf-account-group', route: { name: routeNames.TutorList } },
        { title: 'profile_become_title', icon: 'sbf-find', url: 'https://teach.spitball.co/', notShowFrymo: true },
        { title: 'menuList_my_study_rooms', icon: 'sbf-pc', route: { name: routeNames.StudyRoom } },
        { title: 'menuList_help', icon: 'sbf-help', url: satelliteService.getSatelliteUrlByName('faq') },
        { title: 'menuList_about_spitball',  icon: 'sbf-about', url: satelliteService.getSatelliteUrlByName('about') },
        { title: 'menuList_terms_of_service', icon: 'sbf-terms', url: satelliteService.getSatelliteUrlByName('terms') },
        { title: 'menuList_privacy_policy', icon: 'sbf-privacy', url: satelliteService.getSatelliteUrlByName('privacy') }
      ],
      menuListTeacher:[
        { title: 'menuList_help', icon: 'sbf-help', url: satelliteService.getSatelliteUrlByName('faq') },
        { title: 'menuList_about_spitball', icon: 'sbf-about', url: satelliteService.getSatelliteUrlByName('about') },
        { title: 'menuList_terms_of_service', icon: 'sbf-terms', url: satelliteService.getSatelliteUrlByName('terms') },
        { title: 'menuList_privacy_policy', icon: 'sbf-privacy', url: satelliteService.getSatelliteUrlByName('privacy') },
        { title: 'menuList_referral_spitball', icon: 'sbf-user', action: this.openReferralDialog },
        { title: 'menuList_feedback', icon: 'sbf-feedbackNew', action: this.startIntercom }
      ],
      menuListStudent: [
        { title: 'menuList_account_setting', icon: 'sbf-settings', route: routeNames.Profile },
        { title: 'menuList_my_purchases', icon: 'sbf-cart', route: routeNames.MyPurchases },
        { title: 'menuList_my_studyroom', icon: 'sbf-studyroom-icon', route: routeNames.MyStudyRooms },
        { title: 'menuList_my_sales', icon: 'sbf-my-sales', route: routeNames.MySales },
        { title: 'menuList_help', icon: 'sbf-help', url: satelliteService.getSatelliteUrlByName('faq') }
      ]
    }
  },
  computed: {
    ...mapGetters(['accountUser', 'getUserLoggedInStatus']),
    
    menuListUserType() {      
      let userType = 'default'
      if(this.isLoggedIn) {
        userType = this.user?.isTutor ? 'teacher' : 'student'        
      }
      return this.menuListComponent[userType]
    },
    isMobile() {
      return this.$vuetify.breakpoint.smAndDown;
    },
    user() {
      return { ...this.accountUser };
    },
    isLoggedIn() {
      return this.getUserLoggedInStatus;
    },
    showChangeLanguage() {
      return global.country === 'IL';
    }
  },
  methods: {           
    ...mapActions(['updateReferralDialog', 'logout']),

    changeLanguage(id) {
      LanguageChange.setUserLanguage(id).then(
        () => {
          global.location.reload(true);
        }     
      );
    },
    startIntercom() {
      intercomSettings.showDialog();
    },
    openReferralDialog() {
      this.$emit('closeMenu')
      setTimeout(() => {
        this.updateReferralDialog(true);
      });
    },
    userBalance(balance){
      if(!balance) return
      let balanceFixed = +balance.toFixed()
      return balanceFixed.toLocaleString(`${global.lang}`)
    },
    openLoginDialog() {
      this.$store.commit('setComponent', 'login');
      this.$emit('closeMenu')
    },
    openRegisterTypeDialog() {
      this.$store.commit('setComponent', 'registerType');
      this.$emit('closeMenu')
    }
  },
  created() {
    let currentLocHTML = document.documentElement.lang;
    this.languageChoisesAval = languagesLocales.filter(lan => {
      return lan.locale !== currentLocHTML;
    });
  }
};
</script>

<style lang="less">
@import "../../../../styles/mixin.less";

.menuList {
  width: 280px;
  padding: 0 0 18px;
  @media (max-width: @screen-xs) {
    padding-top: 0;
  }
  .menuListTopPosition {
    width: 280px;
    height: 89px;
    background-image: url('./image/rectangle.png');
    position: absolute;
    background-position: bottom;
  }
  .userMenu_top {
    .uM_top_txts {
      color: #43425d;
      padding-top: 10px;
      .uM_title {
        font-size: 14px;
        font-weight: 600;
        line-height: 1.57;
      }
      .uM_subtitle {
        font-size: 12px;
        font-weight: normal;
        line-height: 1.83;
      }
    }
  }
  .menuListTopNotLogged {
    height: 196px;
    background: #4c59ff;
    border-bottom-left-radius: 26px;
    border-bottom-right-radius: 26px;
    margin-bottom: 10px;
    color: #fff;
    .menuListTopLogo {
      .logo {
        fill: #fff;
        width: 100px;
        height: 28px;
      }
    }
    .menuListTopText {
      font-size: 16px;
      white-space: pre-line;
    }
    .menuListTopBtns {
      display: inline-flex;
      margin-top: 26px;
      
      .menuListTopBtnsUp {
        border: 2px solid #fff !important;
      }
      .menuListTopBtnsIn {
        color:#4c59ff;
      }
    }
  }
  .userMenu_actionsList {
    .v-list-item__action {
      margin: 0 20px 0 0;
      justify-content: center;
    }

  }
  .userMenu_titles {
    font-size: 14px !important;
    color: #43425d;
    line-height: normal !important;
  }
}
.drawerIndex {
  z-index: 210 !important;
}
</style>