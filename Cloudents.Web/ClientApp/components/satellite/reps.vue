<template>
<div class="reps">
  <general-page header="reps-header.png" icon="reps.png">
      
      <div class="reps text-xs-center text-sm-left" v-language:inner>
         <span v-language:inner>reps_help_to_community </span>
          <a href="mailto:work@spitball.co" v-language:inner>reps_email</a>
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


