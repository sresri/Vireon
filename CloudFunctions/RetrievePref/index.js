const functions = require("firebase-functions");
const admin = require("firebase-admin");
admin.initializeApp();
const db = admin.firestore();

const ZONES = ["cliff", "mountain", "beach", "forest", "shore"];

exports.getZoneBasedRecommendations = functions.https.onCall(async (data, context) => {
  const { userId } = data;
  if (!userId) {
    throw new functions.https.HttpsError("invalid-argument", "Missing userId");
  }

  try {
    const userDoc = await db.collection("users").doc(userId).get();
    if (!userDoc.exists) {
      throw new functions.https.HttpsError("not-found", "User not found");
    }

    const userVector = userDoc.data().zonePreferences;
    if (!userVector) {
      throw new functions.https.HttpsError("failed-precondition", "User has no zone preferences");
    }

    const movieSnapshot = await db.collection("movies").get();
    const recommendations = [];

    movieSnapshot.forEach(doc => {
      const data = doc.data();
      const movieVector = data.zoneVector;
      if (!movieVector) return;

      let dot = 0, magUser = 0, magMovie = 0;

      for (const zone of ZONES) {
        const u = userVector[zone] || 0;
        const m = movieVector[zone] || 0;
        dot += u * m;
        magUser += u * u;
        magMovie += m * m;
      }

      const sim = (magUser && magMovie) ? dot / (Math.sqrt(magUser) * Math.sqrt(magMovie)) : 0;

      recommendations.push({
        id: doc.id,
        title: data.title || "Untitled",
        similarity: sim
      });
    });

    recommendations.sort((a, b) => b.similarity - a.similarity);
    return recommendations.slice(0, 5);

  } catch (error) {
    throw new functions.https.HttpsError("internal", error.message);
  }
});
