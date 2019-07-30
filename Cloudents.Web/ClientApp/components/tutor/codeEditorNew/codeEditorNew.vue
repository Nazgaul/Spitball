<template>
    <div>
        <p>aflkjsfdlkjasdflkjasdlkfjasdlkfjaskldjflkasjflkasjflkjasflkjaslkdjafsdlkjfdaslk</p>
        <codemirror></codemirror>
    </div>
</template>

<script>
import { codemirror } from 'vue-codemirror'

import { mapGetters } from 'vuex';

export default {
    data() {
        return {
        }
    },
    components:{
        codemirror
    },
    computed: {
        ...mapGetters(['getIsDarkTheme'])
    },

}
</script>

<style>

</style>
