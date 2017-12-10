<template>
    <div class="uni ml-3 mr-3 mb-3" :key="item.id" @click="$_universitySelected(item)">
        <div class="image elevation-1 mb-3" :style="`background-image: url('${item.image}')`"></div>
        <div>{{item.name}}</div>
    </div>
</template>
<script>
    import { mapActions } from 'vuex'
    export default {
        props: { item: { type: Object, required: true } },
        methods: {
            ...mapActions(['updateUniversity']),
            $_universitySelected(val) {
                this.updateUniversity(val);
            }
        }
    }
</script>