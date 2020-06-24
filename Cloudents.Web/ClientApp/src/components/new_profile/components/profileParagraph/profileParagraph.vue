<template>
  <div class="profileParagraph mt-12">
      <span class="paragraph">
        {{paragraph}}
      </span>
  </div>
</template>

<script>
export default {
    name: 'profileParagraph',
    computed: {
        paragraph() {
            return this.$store.getters.getProfileParagraph
        }
    }
}
</script>

<style lang="less">
.profileParagraph {
    text-align: center;
    max-width: 960px;
    margin: 0 auto;
    color: #363637;
    white-space: break-spaces; // for now
    .paragraph {
        line-height: 1.7;
        font-size: 20px;
    }
}
</style>