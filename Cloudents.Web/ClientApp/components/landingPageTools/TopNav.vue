<template>
    <div class="tutor-list">
        <v-layout align-center justify-center justify-space-between class="tutor-list-header" tag="header">
            <v-flex class="tutor-list-header-left">
                <router-link class="tutor-list-header-logo ml-1" to="/">
                    <logoComponent></logoComponent>
                </router-link>
                <v-icon @click="handleMenuToggle()" class="tutor-list-header-left-menu hidden-md-and-up">sbf-menu</v-icon>
            </v-flex>
            <v-flex class="tutor-list-header-right hidden-sm-and-down">
              <template  v-if="!loggedIn">
                  <button
                    class="tutor-list-header-right-login mr-2"
                    @click="$router.push('../signin')"
                    v-language:inner="'tutorListLanding_topnav_btn_login'"
                  >
                  </button>
                  <button
                    class="tutor-list-header-right-signup mr-3"
                    @click="$router.push('../register')"
                    v-language:inner="'tutorListLanding_topnav_btn_signup'"
                  >
                  </button>

              </template>
              <a @click="changeLanguage()" v-if="!isFrymo" sel="language">
                {{currLanguage !== languageChoisesAval.id? languageChoisesAval.title : ''}}
              </a>
            </v-flex>

            <v-navigation-drawer 
              temporary v-model="drawer" 
              light 
              :right="!isRtl"
              fixed app v-if="isMobileView"
              :class="isRtl ? 'hebrew-drawer' : ''"
              width="280">
                 <menu-list :isAuthUser="loggedIn"></menu-list>
            </v-navigation-drawer>

        </v-layout>
    </div>
</template>

<script>
import logoComponent from '../app/logo/logo.vue';
import LogoSvg from "../../../wwwroot/Images/logo-spitball.svg";
import languagesLocales from "../../services/language/localeLanguage";
import { LanguageChange } from "../../services/language/languageService";
import menuList from "../helpers/menu-list/menu-list.vue";
import { mapGetters } from 'vuex';

export default {
  components: {
    LogoSvg,
    menuList,
    logoComponent
  },
  name: "TopNav",
  data() {
    return {
      languageChoisesAval: [],
      navToggle: false,
      drawer: false,
      isRtl: global.isRtl,
      currLanguage: document.documentElement.lang
    };
  },
  methods: {
    changeLanguage() {
      LanguageChange.setUserLanguage(this.languageChoisesAval.id).then(
        resp => {
          console.log("language responce success", resp);
          global.location.reload(true);
        },
        error => {
          console.log("language error error", error);
        }
      );
    },
    handleMenuToggle() {
      this.drawer = !this.drawer
    }
  },
  computed: {
    ...mapGetters(['accountUser', 'isFrymo']),

    isMobileView() {
      return this.$vuetify.breakpoint.width < 1024;
    },
    loggedIn() {
      return !!this.accountUser
    },
  },
  created() {
    let currentLocHTML = document.documentElement.lang;
    this.languageChoisesAval = languagesLocales.filter(lan => {
      return lan.locale !== currentLocHTML;
    })[0];
  }
};
</script>

<style lang="less">
@import "../../styles/mixin.less";
.tutor-list {
  background: #fff;
  padding: 10px;
  .tutor-list-header {
    max-width: 1500px; 
    margin: 0 auto;
    .tutor-list-header-left {
      // padding: 10px 0;
      display: flex;
      @media (max-width: @screen-sm) {
          justify-content: space-between
      }
      .tutor-list-header-logo {
        div{
          svg {
            vertical-align: -webkit-baseline-middle;
            fill: #43425d;
            &.frymo-logo{
              fill: #378bd3;
            }
            
          }
        } 
      }
      .tutor-list-header-left-menu {
        color: #000;
        font-size: 14px;
        z-index: 99;
      }
    }
    .tutor-list-header-right {
      display: flex;
      justify-content: flex-end;
      align-items: center;

      button {
        text-align: center;
        border-radius: 6px;
        font-size: 14px;
        font-weight: 600;
        outline: none;
        &.tutor-list-header-right-signup {
        padding: 7px 14px;
        margin: 8px;
        background-color: #4c59ff;
        color: rgba(255, 255, 255, 0.87);
      }
      &.tutor-list-header-right-login {
        padding: 7px 20px;
        margin: 5px;
        border: solid 1px #43425d;
        color: #43425d;
        background-color: transparent;
      }
      }
      
      a {
        text-decoration: none;
        font-size: 16px;
        color: #43425d;
        font-weight: bold;
      }
    }
  }
  .v-navigation-drawer {
  z-index: 104;
    &.hebrew-drawer{
      // swap of right and left is going to be done by webpack RTL, so real vals are oposite
      right: 0;
      left: unset;
    }
  }
}
</style>