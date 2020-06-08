<template>
   <v-sheet color="#fff" class="mt-3 mt-sm-5 feedSkeleton">
      <v-skeleton-loader type="list-item-avatar-two-line" max-width="250"></v-skeleton-loader>
      <v-skeleton-loader type="list-item-three-line"></v-skeleton-loader>
      <v-skeleton-loader type="actions"></v-skeleton-loader>
   </v-sheet>
</template>

<style lang="less">
.feedSkeleton{
   border-radius: 12px;
   .v-skeleton-loader__avatar {
      border-radius: 50%;
   }
}
</style>