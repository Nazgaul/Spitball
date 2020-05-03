<template>
   <v-card :color="'grey lighten-1'" class="userPreview" height="100" width="154">
      <span class="name">Brenda Walsh</span>
   </v-card>  
</template>

<script>
export default {

}
</script>

<style lang="less">
.userPreview{
   background-image: linear-gradient(to bottom, rgba(0, 0, 0, 0) 23%, rgba(0, 0, 0, 0.1) 46%, rgba(0, 0, 0, 0.64));
   .name{
      margin-left: 8px;
      font-size: 12px;
      font-weight: 600;
      color: #ffffff;
   }
}
</style>