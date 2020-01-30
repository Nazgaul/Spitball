<template>
  <form class="searchCMP" @submit.prevent="searchQuery" action=".">
    <div class="searchCMP-cont">
       <!-- class="searchCMP-input" -->
      <v-text-field
       
        v-model="search"
      class="searchCMP-input"
        solo
        prepend-inner-icon="sbf-search"
        :placeholder="placeholder"
        autocomplete="off"
        :clear-icon="'sbf-close'"
        hide-details
        clearable
        type="search"/>
    </div>
    <input :value="searchBtnText" type="submit" class="searchCMP-btn" />
  </form>
</template>

<script>
import { mapMutations, mapGetters } from 'vuex';
import {LanguageService } from "../../../../services/language/languageService";

export default {
  props: {
    placeholder: {
      type: String,
      required: false
    }
  },
  data() {
    return {
      search: "",
      searchBtnText: LanguageService.getValueByKey('search_search_btn')
    };
  },
  watch: {
    getSearchStatus(){
        this.search = ""
    }
  },
  computed: {
    ...mapGetters(['getSearchStatus'])
  },
  methods: {
    ...mapMutations(['UPDATE_SEARCH_LOADING']),
    searchQuery() {
      let route = {
        name : "feed"
      };
      if(this.search){
        route.query = { term: this.search };
      }
      this.UPDATE_SEARCH_LOADING(true);
      this.$router.push(route).catch(() => {
        this.UPDATE_SEARCH_LOADING(false);
      });

    }
  }
};
</script>

<style lang="less">
@import "../../../../styles/mixin.less";
.searchCMP {
  overflow: hidden;
  width: 100%;
  height: 100%;
  border-radius: 12px;
  display: flex;
  .searchCMP-cont {
    background: white !important;
    flex: 2;
    height: 100%;
    .searchCMP-input {
      height: 100%;
      .v-input__append-inner {
          .v-icon {
            &.sbf-close {
              font-size: 10px;
            }
          }
        }
      .v-input__control {
        height: 100%;
        min-height: initial;
        justify-content: center;
        .v-input__slot {
          //background: none;
          box-shadow: none;
          //height: 100%;
          //padding-right: 0;
          .v-input__icon {
            i {
              color: #c3c3d0;
            }
          }
          .v-text-field__slot {
            @media (max-width: @screen-xs) {
              font-size: 16px;
            }
            font-size: 18px;
            //font-weight: normal;
            //font-stretch: normal;
            //font-style: normal;
            //letter-spacing: normal;
            color: #a1a3b0;
            input {
              height: 100%;
            }
            
          }
        }
      }
    }
  }
  .searchCMP-btn {
    cursor: pointer;
    color: white;
    font-size: 20px;
    display: flex;
    align-items: center;
    justify-content: center;
    height: 100%;
    background-color: #4c59ff;
    padding: 0 20px;
    @media (max-width: @screen-xs) {
      font-size: 14px;
    }
  }
}
</style>