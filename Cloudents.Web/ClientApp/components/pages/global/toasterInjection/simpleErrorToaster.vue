<template>
    <v-snackbar
        absolute
        top
        color="error"
        :timeout="params.timeout || 3000"
        :value="true"
    >
       {{params.text}}
    </v-snackbar>
</template>


<script>
export default {
    name: 'simpleErrorToaster',
    props: {
        params: {
            type:Object,
            required:true
        }
    },
}
</script>