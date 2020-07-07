<template>
  <div class="profileParagraph pa-2 mt-sm-6 mt-3 px-4">
    <div class="paragraph">
        <span class="paragraphSpan" dir="auto">{{bio | truncate(isOpen, '...', textLimit)}}</span>
    </div>
    <div class="d-none">{{bio | restOfText(isOpen, '...', textLimit)}}</div>
    <span sel="bio_more" v-if="bio && bio.length >= textLimit" @click="isOpen = !isOpen" class="readMore">{{readMoreText}}</span>
  </div>
</template>

<script>
export default {
    name: 'profileParagraph',
    data() {
        return {
            defOpen:false,
        }
    },
    computed: {
        readMoreText() {
            return this.isOpen ? this.$t('profile_read_less') : this.$t('profile_read_more')
        },
        // isMyProfile(){
        //     return this.$store.getters.getIsMyProfile
        // },
        bio() {
            return this.$store.getters.getProfileParagraph
        },
        isMobile() {
            return this.$vuetify.breakpoint.xsOnly
        },
        isOpen :{
            get(){
                return this.defOpen
            },
            set(val){
                this.defOpen = val
            }
        },
        textLimit(){
            return this.isMobile ? 145 : 320;
        },
    },
    filters: {
        truncate(val = '', isOpen, suffix, textLimit){
            if (val.length > textLimit && !isOpen) {
                return val.substring(0, textLimit) +  suffix + ' ';
            } 
            if (val.length > textLimit && isOpen) {
                return val + ' ';
            }
            return val;
        },
        restOfText(val = '', isOpen, suffix, textLimit){
            if (val.length > textLimit && !isOpen) {
                return val.substring(textLimit) ;
            }
            if (val.length > textLimit && isOpen) {
                return '';
            }
        }
    },
}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';

.profileParagraph {
    max-width: 753px;
    margin: 0 auto;
    color: #363637;
    .responsive-property(font-size, 18px, null, 16px);
    .paragraph {
        white-space: pre-line;
        display: contents;
        line-height: 1.7;
        .paragraphSpan {
            display: inline-block;
        }
    }
    .readMore {
      font-weight: 600;
      cursor: pointer;
    }
}
</style>