<template>
  <div class="partner">
    <general-page header="partners-header.png" icon="partners-icon.png">
          <div class="partner text-xs-center text-sm-left">
              <span v-language:inner>partner_join_the_community</span>&nbsp;<a href="mailto:partner@spitball.co">partner@spitball.co</a>
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


<style src="./partner.less" lang="less"></style>

