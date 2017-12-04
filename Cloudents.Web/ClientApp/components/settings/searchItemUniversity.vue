<template>
    <div class="uni" :key="item.id" @click="$_universitySelected(item)">
        <img :src="item.image" alt="">
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