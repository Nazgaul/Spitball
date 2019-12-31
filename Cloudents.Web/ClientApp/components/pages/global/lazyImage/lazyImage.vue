<template>
    <img :src="srcImage" />
</template>

<script>
export default {
  props: {
    src: {},
    element: {}
  },
  data: () => ({ observer: null, intersected: false }),
  computed: {
    srcImage() {
        return this.intersected ? this.src : '';
    }
  },
  mounted() {
    if ('IntersectionObserver' in window) {
        this.observer = new IntersectionObserver((entries) => {
          const image = entries[0];
          if (image.isIntersecting) {
              this.intersected = true;
              this.observer.disconnect();
          }
        });
        this.observer.observe(this.element);
    }
  },
  destroyed() {
    this.observer.disconnect();
  }
}
</script>