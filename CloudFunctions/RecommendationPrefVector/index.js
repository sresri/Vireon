
const functions = require("firebase-functions");
const admin = require("firebase-admin");
admin.initializeApp();
const db = admin.firestore();

const ZONES = ["cliff", "mountain", "beach", "forest", "shore"];

exports.updateUserZonePreferences = functions.https.onCall(async (data, context) => {
  const { userId, movieId } = data;
  if (!userId || !movieId) {
    throw new functions.https.HttpsError("invalid-argument", "Missing userId or movieId");
  }

  try {
    const movieDoc = await db.collection("movies").doc(movieId).get();
    if (!movieDoc.exists) {
      throw new functions.https.HttpsError("not-found", "Movie not found");
    }

    const zoneVector = movieDoc.data().zoneVector;
    const userRef = db.collection("users").doc(userId);
    const userDoc = await userRef.get();

    let userVector = userDoc.exists && userDoc.data().zonePreferences
      ? userDoc.data().zonePreferences
      : ZONES.reduce((obj, zone) => ({ ...obj, [zone]: 0 }), {});

    const alpha = 0.7;
    const updatedVector = {};

    for (const zone of ZONES) {
      const u = userVector[zone] || 0;
      const m = zoneVector[zone] || 0;
      updatedVector[zone] = alpha * u + (1 - alpha) * m;
    }

    await userRef.set({ zonePreferences: updatedVector }, { merge: true });
    return { success: true, updatedVector };

  } catch (error) {
    throw new functions.https.HttpsError("internal", error.message);
  }
});
