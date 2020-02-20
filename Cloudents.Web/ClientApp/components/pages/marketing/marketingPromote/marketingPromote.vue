<template>
    <div>
        <marketingActions #closeIcon :resource="$route.name">
            <close />
        </marketingActions>
    </div>
</template>

<script>
import marketingActions from '../marketingActions/marketingActions.vue';
import close  from "../../../../font-icon/close.svg";

export default {
    components: {
        marketingActions,
        close
    }
}
</script>