<template>
  <div class="profileParagraph text-left text-sm-center pa-2 px-4" :class="{'mt-2': !isMyProfile}">
    <div class="paragraph">
        {{bio | truncate(isOpen, '...', textLimit)}}
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
        isMyProfile(){
            return this.$store.getters.getIsMyProfile
        },
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
            return this.isMobile ? 68 : 250;
        },
    },
    filters: {
        truncate(val, isOpen, suffix, textLimit){
            if (val.length > textLimit && !isOpen) {
                return val.substring(0, textLimit) +  suffix + ' ';
            } 
            if (val.length > textLimit && isOpen) {
                return val + ' ';
            }
            return val;
        },
        restOfText(val, isOpen, suffix, textLimit){
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
    max-width: 830px;
    margin: 0 auto;
    color: #363637;
    .paragraph {
        display: contents;
        line-height: 1.7;
        font-size: 20px;
        @media (max-width: @screen-xs) {
            font-size: 16px;
        }
    }
}
</style>