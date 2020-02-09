<template>
    <v-row class="spitballTips" dense>
        <v-col cols="12" class="pa-0 d-flex justify-space-between">
            <div class="tipTitle">Tips from spitball blog</div>
            <router-link class="seeALl" :to="{ name: 'routeName'}">see all</router-link>
        </v-col>
        <v-col cols="12">
          <!-- ... -->
        </v-col>
    </v-row>
</template>
<script>
export default {
  name: "spitballTips",
  data: () => ({

  })
}
</script>
<style lang="less">
.spitballTips {
  padding: 12px 16px;
  background: white;
  box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
  border-radius: 8px;

  @media (max-width: 599px) {
    box-shadow: none;
  }

  .tipTitle {
    color: #43425d;
    font-weight: 600;
    font-size: 16px;
  }
  .seeALl {
    color: #4c59ff;
  }
}
</style>
