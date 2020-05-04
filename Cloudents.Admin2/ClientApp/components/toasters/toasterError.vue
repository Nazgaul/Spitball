<template>
   <v-snackbar :value="true" color="error" top right @input="$store.commit('setSnackbar')">
       {{ params }}
      <v-btn
        color="#fff"
        text
        @click="snackbar = false"
      >
        Close
      </v-btn>
    </v-snackbar>
</template>

<script>

export default {
    name: "componentInjection",
    props: {
        params: {}
    }
}
</script>