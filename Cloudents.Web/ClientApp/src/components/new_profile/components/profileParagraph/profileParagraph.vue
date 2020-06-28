<template>
  <div class="profileParagraph text-left text-sm-center pa-2 pa-sm-0" :class="{'mt-9': !isMyProfile}">
      <span class="paragraph">
        {{paragraph}}
      </span>
  </div>
</template>

<script>
export default {
    name: 'profileParagraph',
    computed: {
        isMyProfile(){
            return this.$store.getters.getIsMyProfile
        },
        paragraph() {
            return this.$store.getters.getProfileParagraph
        }
    }
}
</script>

<style lang="less">
.profileParagraph {
    max-width: 960px;
    margin: 0 auto;
    color: #363637;
    // white-space: break-spaces; // for now
    .paragraph {
        line-height: 1.7;
        font-size: 20px;
    }
}
</style>