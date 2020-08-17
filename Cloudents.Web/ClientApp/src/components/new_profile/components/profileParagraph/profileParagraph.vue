<template>
  <div id="profileParagraph" class="profileParagraph pa-2 mt-sm-6 mt-3 px-4" dir="auto">
    <div class="paragraph">
        <span class="paragraphSpan">{{bio}}</span>
    </div>
  </div>
</template>

<script>
export default {
    name: 'profileParagraph',
    computed: {
        bio() {
            return this.$store.getters.getProfileTempBio || this.$store.getters.getProfileParagraph
        },
    }
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
    }
}
</style>