<template>
    <v-snackbar
        absolute
        top
        color="error"
        :timeout="params.timeout || 3000"
        :value="true"
    >
    <span v-html="params.text"></span>
    </v-snackbar>
</template>


<script>

export default {
    name: 'errorLinkToaster',
    props: {
        params: {
            type:Object,
            required:true
        }
    },
}
</script>