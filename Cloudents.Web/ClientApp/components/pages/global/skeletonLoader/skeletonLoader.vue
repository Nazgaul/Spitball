<template>
  <v-sheet
    color="#fff"
    class="pb-2 skeletonWarp"
  >
    <v-skeleton-loader
      class="mx-auto"
      >
        <v-boilerplate
          class="elevation-0"
          max-width="250"
          type="list-item-avatar-two-line"
          :boilerplate="false"
        ></v-boilerplate>

        <v-boilerplate
          class="elevation-0"
          max-width="500"
          type="list-item-three-line"
          :boilerplate="false"
        ></v-boilerplate>

        <v-boilerplate
          class="elevation-0 mt-4 pl-4"
          max-width="250"
          type="text"
          :boilerplate="false"
        ></v-boilerplate>

    </v-skeleton-loader>
  </v-sheet>
</template>

<script>
 export default {
    // Vuetify components provide
    // a theme variable that is
    // used to determine dark
    inject: ['theme'],

    components: {
      // Create a new component that
      // extends v-skeleton-loader
      VBoilerplate: {
        functional: true,

        render (h, { data, props, children }) {
          return h('v-skeleton-loader', {
            ...data,
            props: {
              boilerplate: true,
              elevation: 2,
              ...props,
            },
          }, children)
        },
      },
    },
  }
</script>

<style lang="less">
  .skeletonWarp {
    border-radius: 8px;

    .v-skeleton-loader__avatar {
      border-radius: 50%; // border-radius overwrite from parents
    }
    .v-skeleton-loader__text {
      margin-bottom: 10px;
    }
  }
</style>