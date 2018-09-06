<template>
<div class="reps">
  <general-page header="reps-header.png" icon="reps.png">
      
      <div class="reps text-xs-center text-sm-left">
        <span v-language:inner>reps_help_to_community</span> &nbsp;
        <a href="mailto:work@spitball.co" v-language:inner>work@spitball.co</a>
      </div>
  </general-page>
</div>

</template>

<script>
import generalPage from "./generalImage.vue";
export default {
  components: {generalPage}
}
</script>

<style src="./reps.less" lang="less"></style>


