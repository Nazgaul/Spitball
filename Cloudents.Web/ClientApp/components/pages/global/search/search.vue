<template>
  <div class="searchCMP">
    <div class="searchCMP-cont">
      <v-text-field
        class="searchCMP-input"
        v-model="search"
        @keyup.enter="searchQuery"
        solo
        prepend-inner-icon="sbf-search"
        :placeholder="placeholder"
        autocomplete="off"
        :clear-icon="'sbf-close'"
        hide-details
        clearable
        type="search"/>
    </div>
    <div @click="searchQuery" class="searchCMP-btn" v-language:inner="'search_search_btn'" />
  </div>
</template>

<script>
import { mapMutations, mapGetters } from 'vuex';

export default {
  props: {
    placeholder: {
      type: String,
      required: false
    }
  },
  data() {
    return {
      search: ""
    };
  },
  watch: {
    getSearchStatus(newVal,oldVal){
        this.search = ""
    }
  },
  computed: {
    ...mapGetters(['getSearchStatus'])
  },
  methods: {
    ...mapMutations(['UPDATE_SEARCH_LOADING']),
    searchQuery() {
      this.UPDATE_SEARCH_LOADING(true);
      if(this.search){
        this.$router.push({ path: "/feed", query: { term: this.search } });
        }else{
        this.$router.push({ path: "/feed"});  
      }
      this.$nextTick(() => {
        setTimeout(()=>{
            this.UPDATE_SEARCH_LOADING(false);
        }, 200);
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
        .v-input__slot {
          background: none;
          box-shadow: none;
          height: 100%;
          padding-right: 0;
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
            font-weight: normal;
            font-stretch: normal;
            font-style: normal;
            letter-spacing: normal;
            color: #a1a3b0;
            
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